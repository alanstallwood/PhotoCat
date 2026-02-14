using PhotoCat.Application.Photos.AddPhoto;
using PhotoCat.Domain.Photos;
using PhotoCat.Infrastructure.Metadata;

namespace PhotoCat.Application.Photos;

public sealed class AddPhotoHandler
{
    private readonly IExifExtractor _exifExtractor;
    private readonly IPhotoRepository _photoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddPhotoHandler(IExifExtractor exifExtractor, IPhotoRepository photoRepository, IUnitOfWork unitOfWork)
    {
        _exifExtractor = exifExtractor;
        _photoRepository = photoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddPhotoCommand command, CancellationToken ct = default)
    {
        if (!File.Exists(command.FilePath))
            throw new FileNotFoundException(
                $"File not found: {command.FilePath}");

        var metadata = _exifExtractor.Extract(command.FilePath); 

        // Create the aggregate using the factory
        var photo = Photo.Create(
            command.FileName,
            command.FilePath,
            command.FileFormat,
            command.SizeBytes,
            command.Checksum,
            command.Tags,
            metadata);

        // Add to repository
        await _photoRepository.AddAsync(photo, ct);

        // Commit transaction / save changes
        await _unitOfWork.SaveChangesAsync(ct);

        return photo.Id; // return the new ID if needed
    }
}
