using MediatR;
using PhotoCat.Application;
using PhotoCat.Application.Exceptions;
using PhotoCat.Application.Interfaces;
using PhotoCat.Application.Photos;
using PhotoCat.Application.Photos.AddPhoto;
using PhotoCat.Application.Photos.DeletePhotoFile;
using PhotoCat.Domain.Services;
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

    public async Task RunFullScanAsync(CancellationToken ct)
    {
        var filesOnDisk = Directory
            .EnumerateFiles(_basePath, "*.*", SearchOption.AllDirectories)
            .ToList();

        if (!filesOnDisk.Any())
        {
            _logger.LogInformation("No new files found to sync.");
            return;
        }

        var newFiles = new List<(string Path, byte[] Checksum)>();

        // Step 1: Compute checksum and filter out duplicates
        foreach (var file in filesOnDisk)
        {
            var checksum = await _checksumService.CalculateAsync(file, ct);

            var existingNewFile = newFiles.Any(f => f.Checksum.SequenceEqual(checksum));
            if(existingNewFile)
            {
                continue; // Skip if already in new files list
            }

            var existingPhotoRecord = await CheckForExistingPhotoFileAndLogIfFound(file, ct);
            if (existingPhotoRecord)
            {
                continue;
            }

            newFiles.Add((file, checksum));
        }

        const int batchSize = 50;
        foreach (var chunk in newFiles.Chunk(batchSize))
        {
            var command = new AddPhotoCommand([.. chunk.Select(c => c.Path)]);

            try
            {
                var photoId = await _mediator.Send(command, ct);

                _logger.LogInformation($"Processed batch, New or modified photos: {photoId}");
            }
            catch (AllFilesAlreadyExistException)
            {
                _logger.LogInformation("All files in batch already exist. Skipping.");
            }
            catch (NoFilesProvidedException)
            {
                _logger.LogInformation("No valid files in batch. Skipping.");
            }
        }

        // Step 4: Handle missing files (soft-delete)
        var allDbPhotoFileFullPaths = await _photoRepository.GetAllPhotoFileFullPathsAsync(ct);
        foreach (var dbFile in allDbPhotoFileFullPaths)
        {
            if (!File.Exists(dbFile.FullFilePath))
            {
                await _mediator.Send(new DeletePhotoFileCommand(dbFile.PhotoId, dbFile.FileId), ct);
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

    private static List<List<(string Path, byte[] Checksum)>> GroupFilesByKeyCandidate(List<(string Path, byte[] Checksum)> files)
    {
        var allFiles = new List<(string Path, string Base, byte[] Checksum)>();
        foreach (var f in files)
        {
            var file = new FileInfo(f.Path);
            var keyCandidate = GroupKeyService.NormalizeAndRemoveModifiers(file.Name);
            allFiles.Add((f.Path, keyCandidate, f.Checksum));
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

    private async Task<bool> CheckForExistingPhotoFileAndLogIfFound(string path, CancellationToken ct = default)
    {
        var checksum = await _checksumService.CalculateAsync(path, ct);
        var exists = await _photoRepository.FileChecksumExistsAsync(checksum, ct);
        if (exists)
        {
            _logger.LogInformation($"Duplicate file skipped: {path} (checksum {checksum})");
        }
        return exists;
    }

    [GeneratedRegex(@"[^a-z0-9]")]
    private static partial Regex NonAlphaNumberic();
}
