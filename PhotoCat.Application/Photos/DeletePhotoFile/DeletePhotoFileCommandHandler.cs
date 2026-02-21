using MediatR;
using PhotoCat.Application.Exceptions;

namespace PhotoCat.Application.Photos.DeletePhotoFile;

public sealed class DeletePhotoFileCommandHandler(IPhotoRepository photoRepository) : IRequestHandler<DeletePhotoFileCommand>
{
    private readonly IPhotoRepository _photoRepository = photoRepository;
    public async Task Handle(DeletePhotoFileCommand request, CancellationToken ct)
    {
        var photo = await _photoRepository.GetByIdAsync(request.PhotoId, ct);
        if (photo == null)
        {
            throw new PhotoNotFoundException(request.PhotoId);
        }

        photo.SoftDeleteFile(request.PhotoFileId);

        await _photoRepository.UpdateAsync(photo, ct);
    }
}
