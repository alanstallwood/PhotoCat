using MediatR;
using PhotoCat.Application.Exceptions;
using PhotoCat.Application.Interfaces;
using PhotoCat.Application.Photos.AddPhoto;
using PhotoCat.Domain.Photos;
using PhotoCat.Infrastructure.Metadata;

namespace PhotoCat.Application.Photos;

public sealed class AddPhotoCommandHandler(IExifExtractor exifExtractor, IChecksumService checksumService, 
                                        IFileTypeDetector fileTypeDetector, IPhotoRepository photoRepository) 
    : IRequestHandler<AddPhotoCommand, Guid>
{
    private readonly IExifExtractor _exifExtractor = exifExtractor;
    private readonly IChecksumService _checksumService = checksumService;
    private readonly IFileTypeDetector _fileTypeDetector = fileTypeDetector;
    private readonly IPhotoRepository _photoRepository = photoRepository;

    public async Task<Guid> Handle(AddPhotoCommand request, CancellationToken ct = default)
    {
        if (request.FullFilePaths == null || request.FullFilePaths.Count == 0)
            throw new NoFilesProvidedException();

        var newFiles = await BuildNewFilesAsync(request.FullFilePaths, ct);

        if (newFiles.Count == 0)
            throw new AllFilesAlreadyExistException();

        // Create the aggregate using the factory
        var photo = Photo.Create(
            newFiles,
            request.Tags);

        await _photoRepository.AddAsync(photo, ct);

        return photo.Id;
    }

    private async Task<List<NewFileDto>> BuildNewFilesAsync(IReadOnlyCollection<string> fullFilePaths, CancellationToken ct)
    {
        var results = new List<NewFileDto>();

        foreach (var fullFilePath in fullFilePaths)
        {
            if (!File.Exists(fullFilePath))
                throw new FileNotFoundApplicationException(fullFilePath);

            var checksum = await _checksumService.CalculateAsync(fullFilePath, ct);

            if (await _photoRepository.FileChecksumExistsAsync(checksum, ct))
                continue;

            var metadata = _exifExtractor.Extract(fullFilePath);
            var fileType = _fileTypeDetector.Detect(fullFilePath);
            var fileInfo = new FileInfo(fullFilePath);

            results.Add(new NewFileDto(
                fileInfo.Name,
                fileInfo.DirectoryName!,
                fileType,
                checksum,
                fileInfo.Length,
                metadata));
        }

        return results;
    }
}
