using PhotoCat.Application.Interfaces;
using PhotoCat.Application.Photos.AddPhoto;
using PhotoCat.Domain.Photos;
using PhotoCat.Infrastructure.Metadata;
using PhotoCat.Infrastructure.Photos;

namespace PhotoCat.Application.Photos;

public sealed class AddPhotoHandler
{
    private readonly IExifExtractor _exifExtractor;
    private readonly IChecksumService _checksumService;
    private readonly IFileTypeDetector _fileTypeDetector;
    private readonly IPhotoRepository _photoRepository;

    public AddPhotoHandler(IExifExtractor exifExtractor, IChecksumService checksumService, IFileTypeDetector fileTypeDetector, IPhotoRepository photoRepository)
    {
        _exifExtractor = exifExtractor;
        _checksumService = checksumService;
        _fileTypeDetector = fileTypeDetector;
        _photoRepository = photoRepository;
    }

    public async Task<AddPhotoResult> Handle(AddPhotoCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.FilePath))
            throw new ArgumentException("FilePath is required.");

        if (!File.Exists(command.FilePath))
            throw new FileNotFoundException(
                $"File not found: {command.FilePath}");

        var checksum = await _checksumService.CalculateAsync(command.FilePath, ct);
        var existing = await _photoRepository
            .GetByChecksumAsync(checksum, ct);

        if (existing is not null)
        {
            return AddPhotoResult.AlreadyExists(existing.Id);
        }

        //var fileInfo = new FileInfo(command.FilePath);
        //var fileType = _fileTypeDetector.Detect(command.FilePath);
        //TODO: Move this to PhotoFile
        var metadata = _exifExtractor.Extract(command.FilePath);


        // Create the aggregate using the factory
        var photo = Photo.Create(
            metadata,
            command.Tags);

        await _photoRepository.AddAsync(photo, ct);

        return AddPhotoResult.Created(photo.Id);
    }
}
