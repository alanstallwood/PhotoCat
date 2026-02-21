using MediatR;
using PhotoCat.Application.Exceptions;

namespace PhotoCat.Application.Photos.RestorePhotoFile;

public sealed class RestorePhotoFileCommandHandler(IPhotoRepository photoRepository) : IRequestHandler<RestorePhotoFileCommand>
{
    private readonly IPhotoRepository _photoRepository = photoRepository;
    public async Task Handle(RestorePhotoFileCommand request, CancellationToken ct)
    {
        var photo = await _photoRepository.GetByIdAsync(request.PhotoId, ct);
        if (photo == null)
        {
            throw new PhotoNotFoundException(request.PhotoId);
        }
        photo.RestoreFile(request.PhotoFileId);
        await _photoRepository.UpdateAsync(photo, ct);
    }
}
