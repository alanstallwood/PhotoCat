using MediatR;
using PhotoCat.Application;
using PhotoCat.Application.Interfaces;
using PhotoCat.Application.Photos;
using PhotoCat.Application.Photos.AddPhoto;
using PhotoCat.Application.Photos.DeletePhotoFile;
using System.Text.RegularExpressions;

namespace PhotoCat.FileSystemScanner.Services;

public partial class PhotoSyncWorker(IChecksumService checksumService, IPhotoRepository photoRepository, 
    IDomainEventDispatcher events, IMediator mediator, ILogger<PhotoSyncWorker> logger)
{
    private readonly IChecksumService _checksumService = checksumService;
    private readonly IPhotoRepository _photoRepository = photoRepository;
    private readonly IDomainEventDispatcher _events = events;
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<PhotoSyncWorker> _logger = logger;

    public async Task RunFullScanAsync()
    {
        var filesOnDisk = Directory
            .EnumerateFiles(_basePath, "*.*", SearchOption.AllDirectories)
            .ToList();

        var newFiles = new List<(string Path, byte[] Checksum)>();

        // Step 1: Compute checksum and filter out duplicates
        foreach (var file in filesOnDisk)
        {
            var checksum = await _checksumService.CalculateAsync(file);

            var existingNewFile = newFiles.Any(f => f.Checksum.SequenceEqual(checksum));
            if(existingNewFile)
            {
                continue; // Skip if already in new files list
            }

            var existingPhotoRecord = await CheckForExistingPhotoFileAndLogIfFound(file);
            if (existingPhotoRecord)
            {
                continue;
            }

            newFiles.Add((file, checksum));
        }

        // Step 2: Group by normalized filename
        var groups = GroupFilesByBase(newFiles);

        // Step 3: Send batched AddPhotoCommand per group
        foreach (var group in groups)
        {
            var command = new AddPhotoCommand(
                [.. group.Select(f => f.Path)]
            );
            await _mediator.Send(command);
        }

        // Step 4: Handle missing files (soft-delete)
        var dbFiles = await _photoRepository.GetAllPhotoFilesAsync();
        foreach (var dbFile in dbFiles)
        {
            if (!File.Exists(dbFile.Path))
            {
                await _mediator.Send(new DeletePhotoFileCommand(dbFile.PhotoId, dbFile.Id));
            }
        }
    }

    public async Task HandleNewFileAsync(string path)
    {
        var existingPhotoRecord = await CheckForExistingPhotoFileAndLogIfFound(path);
        if (existingPhotoRecord)
        {
            return;
        }

        // Single file batch for real-time
        var command = new AddPhotoCommand([path]);

        await _mediator.Send(command);
    }

    private List<List<(string Path, byte[] Checksum)>> GroupFilesByBase(List<(string Path, byte[] Checksum)> files)
    {
        var allFiles = new List<(string Path, string Base, byte[] Checksum)>();
        foreach (var f in files)
        {
            var baseName = NormalizeFileName(Path.GetFileNameWithoutExtension(f.Path));
            allFiles.Add((f.Path, baseName, f.Checksum));
        }

        var groups = new List<List<(string Path, byte[] Checksum)>>();

        while (allFiles.Count != 0)
        {
            var file = allFiles[0];
            var group = allFiles
                .Where(f => f.Base.Contains(file.Base) || file.Base.Contains(f.Base))
                .Select(f => (f.Path, f.Checksum))
                .ToList();

            groups.Add(group);
            allFiles.RemoveAll(f => group.Any(g => g.Path == f.Path));
        }

        return groups;
    }

    private async Task<bool> CheckForExistingPhotoFileAndLogIfFound(string path)
    {
        var checksum = await _checksumService.CalculateAsync(path);
        var exists = await _photoRepository.FileChecksumExistsAsync(checksum);
        if (exists)
        {
            _logger.LogInformation($"Duplicate file skipped: {path} (checksum {checksum})");
        }
        return exists;
    }

    string NormalizeFileName(string filename)
    {
        // Remove extension
        var name = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();

        // Remove non-alphanumeric characters
        name = NonAlphaNumberic().Replace(name, "");

        return name;
    }

    [GeneratedRegex(@"[^a-z0-9]")]
    private static partial Regex NonAlphaNumberic();
}
