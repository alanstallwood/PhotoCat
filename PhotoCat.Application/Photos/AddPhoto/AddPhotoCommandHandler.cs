using MediatR;
using PhotoCat.Application.Exceptions;
using PhotoCat.Application.Interfaces;
using PhotoCat.Application.Photos.AddPhoto;
using PhotoCat.Domain.Photos;
using PhotoCat.Domain.Services;
using PhotoCat.Infrastructure.Metadata;

namespace PhotoCat.Application.Photos;

public sealed class AddPhotoCommandHandler(IExifExtractor exifExtractor, IChecksumService checksumService, 
                                        IFileTypeDetector fileTypeDetector, IPhotoRepository photoRepository) 
    : IRequestHandler<AddPhotoCommand, IEnumerable<Guid>>
{
    private readonly IExifExtractor _exifExtractor = exifExtractor;
    private readonly IChecksumService _checksumService = checksumService;
    private readonly IFileTypeDetector _fileTypeDetector = fileTypeDetector;
    private readonly IPhotoRepository _photoRepository = photoRepository;

    public async Task<IEnumerable<Guid>> Handle(AddPhotoCommand request, CancellationToken ct = default)
    {
        if (request.FullFilePaths == null || request.FullFilePaths.Count == 0)
            throw new NoFilesProvidedException();

        var newFiles = await BuildNewFilesAsync(request.FullFilePaths, ct);

        if (newFiles.Count == 0)
            throw new AllFilesAlreadyExistException();

        var newAndModifiedPhotoIds = new List<Guid>();
        var groupedFiles = GroupNewFiles(newFiles);
        foreach (var group in groupedFiles)
        {
            // Find existing Photo by GroupKey (or evolved keys)
            var existingPhoto = await _photoRepository.GetByGroupKeyAsync(group.GroupKey, ct);

            if (existingPhoto != null)
            {
                foreach (var file in group.Files)
                    existingPhoto.AddFile(file);

                await _photoRepository.UpdateAsync(existingPhoto, ct);
                newAndModifiedPhotoIds.Add(existingPhoto.Id);
            }
            else
            {
                // Create new Photo aggregate
                var photo = Photo.Create(group.Files.ToList(), request.Tags);
                await _photoRepository.AddAsync(photo, ct);
                newAndModifiedPhotoIds.Add(photo.Id);
            }
        }

        return newAndModifiedPhotoIds;
    }

    private async Task<List<(NewFileDto File, string KeyCandidate)>> BuildNewFilesAsync(IReadOnlyCollection<string> fullFilePaths, CancellationToken ct)
    {
        var results = new List<(NewFileDto File, string KeyCandidate)>();

        foreach (var fullFilePath in fullFilePaths)
        {
            if (!File.Exists(fullFilePath))
                continue;

            var checksum = await _checksumService.CalculateAsync(fullFilePath, ct);

            if (await _photoRepository.FileChecksumExistsAsync(checksum, ct))
                continue;

            var metadata = _exifExtractor.Extract(fullFilePath);
            var fileType = _fileTypeDetector.Detect(fullFilePath);
            var fileInfo = new FileInfo(fullFilePath);
            var keyCandidate = GroupKeyService.NormalizeAndRemoveModifiers(fileInfo.Name);

            results.Add((new NewFileDto(
                fileInfo.Name,
                fileInfo.DirectoryName!,
                fileType,
                checksum,
                fileInfo.Length,
                metadata), keyCandidate));
        }

        return results;
    }

    private static List<LogicalPhotoGroup> GroupNewFiles(IEnumerable<(NewFileDto File, string KeyCandidate)> files)
    {
        var groups = new List<LogicalPhotoGroup>();

        foreach (var (file, keyCandidate) in files)
        {
            // Find all groups this file could belong to
            var matchingGroups = groups
                .Where(g => GroupKeyService.BelongToSameLogicalPhoto(g.GroupKey, keyCandidate))
                .ToList();

            if (matchingGroups.Count == 0)
            {
                groups.Add(new LogicalPhotoGroup(file, keyCandidate));
                continue;
            }

            var primary = matchingGroups.First();
            primary.TryAdd(file, keyCandidate);

            // Merge overlapping groups
            foreach (var extra in matchingGroups.Skip(1))
            {
                primary.Merge(extra);
                groups.Remove(extra);
            }
        }

        return groups;
    }
}
