using MediatR;
using PhotoCat.Application.Exceptions;

namespace PhotoCat.Application.Photos.DeletePhoto;

public sealed class DeletePhotoCommandHandler(IPhotoRepository photoRepository) : IRequestHandler<DeletePhotoCommand>
{
    private readonly IPhotoRepository _photoRepository = photoRepository;
    public async Task Handle(DeletePhotoCommand request, CancellationToken ct)
    {
        var photo = await _photoRepository.GetByIdAsync(request.PhotoId, ct);
        if (photo == null)
        {
            throw new PhotoNotFoundException(request.PhotoId);
        }

        photo.SoftDelete();
        await _photoRepository.UpdateAsync(photo, ct);
    }
}
