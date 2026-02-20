using MediatR;
using PhotoCat.Application.Exceptions;

namespace PhotoCat.Application.Photos.AddTag
{
    public sealed class AddTagCommandHandler(IPhotoRepository photoRepository) : IRequestHandler<AddTagCommand>
    {
        private readonly IPhotoRepository _photoRepository = photoRepository;

        public async Task Handle(AddTagCommand request, CancellationToken ct)
        {
            var photo = await _photoRepository.GetByIdAsync(request.PhotoId, ct);
            if (photo == null)
            {
                throw new PhotoNotFoundException(request.PhotoId);
            }

            photo.AddTag(request.Tag);
            await _photoRepository.UpdateAsync(photo, ct);
        }
    }
}
