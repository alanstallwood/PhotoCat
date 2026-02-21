using MediatR;

namespace PhotoCat.Application.Photos.DeletePhoto;

public sealed class DeletePhotoCommand(Guid photoId) : IRequest
{
    public Guid PhotoId { get; set; } = photoId;
}
