using MediatR;
using PhotoCat.Application.Exceptions;

namespace PhotoCat.Application.Photos.RestorePhoto;

public sealed class RestorePhotoCommandHandler(IPhotoRepository photoRepository) : IRequestHandler<RestorePhotoCommand>
{
    private readonly IPhotoRepository _photoRepository = photoRepository;

    public async Task Handle(RestorePhotoCommand request, CancellationToken ct)
    {
        var photo = await _photoRepository.GetByIdAsync(request.PhotoId, ct);
        if (photo == null)
        {
            throw new PhotoNotFoundException(request.PhotoId);
        }

        photo.Restore();
        await _photoRepository.UpdateAsync(photo, ct);
    }
}
