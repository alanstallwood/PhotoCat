using PhotoCat.Application.Exceptions;

namespace PhotoCat.Application.Photos.RemoveTag
{
    public sealed class RemoveTagCommandHandler(IPhotoRepository photoRepository) : MediatR.IRequestHandler<RemoveTagCommand>
    {
        private readonly IPhotoRepository _photoRepository = photoRepository;
        public async Task Handle(RemoveTagCommand request, CancellationToken ct)
        {
            var photo = await _photoRepository.GetByIdAsync(request.PhotoId, ct);
            if (photo == null)
            {
                throw new PhotoNotFoundException(request.PhotoId);
            }

            photo.RemoveTag(request.Tag);
            await _photoRepository.UpdateAsync(photo, ct);
        }
    }
}
