using PhotoCat.Application.Photos.AddPhoto;
using PhotoCat.Domain.Photos;
using PhotoCat.Infrastructure.Metadata;
using PhotoCat.Infrastructure.Photos;

namespace PhotoCat.Application.Photos;

public sealed class AddPhotoHandler
{
    private readonly IExifExtractor _exifExtractor;
    private readonly IChecksumService _checksumService;
    private readonly IPhotoRepository _photoRepository;

    public AddPhotoHandler(IExifExtractor exifExtractor, IChecksumService checksumService, IPhotoRepository photoRepository)
    {
        _exifExtractor = exifExtractor;
        _checksumService = checksumService;
        _photoRepository = photoRepository;
    }

    public async Task<AddPhotoResult> Handle(AddPhotoCommand command, CancellationToken ct = default)
    {
        if (!File.Exists(command.FilePath))
            throw new FileNotFoundException(
                $"File not found: {command.FilePath}");

        var metadata = _exifExtractor.Extract(command.FilePath);

        var hash = await _checksumService.CalculateAsync(command.FilePath, ct);

        // Create the aggregate using the factory
        var photo = Photo.Create(
            command.FileName,
            command.FilePath,
            command.FileFormat,
            command.SizeBytes,
            hash,
            command.Tags,
            metadata);

        var result = await _photoRepository.AddIfNotExistsAsync(photo, ct);

        return result;
    }
}
