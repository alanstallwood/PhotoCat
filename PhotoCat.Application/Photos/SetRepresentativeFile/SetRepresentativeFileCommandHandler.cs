using MediatR;
using PhotoCat.Application.Exceptions;

namespace PhotoCat.Application.Photos.SetRepresentativeFile;

public sealed class SetRepresentativeFileCommandHandler(IPhotoRepository photoRepository) : IRequestHandler<SetRepresentativeFileCommand>
{
    private readonly IPhotoRepository _photoRepository = photoRepository;
    public async Task Handle(SetRepresentativeFileCommand request, CancellationToken ct)
    {
        var photo = await _photoRepository.GetByIdAsync(request.PhotoId, ct);
        if (photo == null)
        {
            throw new PhotoNotFoundException(request.PhotoId);
        }

        photo.SetRepresentativeFile(request.PhotoId);
        await _photoRepository.UpdateAsync(photo, ct);
    }
}
