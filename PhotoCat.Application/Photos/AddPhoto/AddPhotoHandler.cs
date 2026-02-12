using PhotoCat.Application.Photos.AddPhoto;
using PhotoCat.Domain.Photos;

namespace PhotoCat.Application.Photos;

public sealed class AddPhotoHandler
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddPhotoHandler(IPhotoRepository photoRepository, IUnitOfWork unitOfWork)
    {
        _photoRepository = photoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddPhotoCommand command, CancellationToken ct = default)
    {
        // Create the aggregate using the factory
        var photo = Photo.Create(
            command.FileName,
            command.FilePath,
            command.FileFormat,
            command.SizeBytes,
            command.Checksum,
            command.DateTaken,
            command.Tags);

        // Add to repository
        await _photoRepository.AddAsync(photo, ct);

        // Commit transaction / save changes
        await _unitOfWork.SaveChangesAsync(ct);

        return photo.Id; // return the new ID if needed
    }
}
