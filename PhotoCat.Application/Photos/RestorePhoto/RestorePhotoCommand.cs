using MediatR;

namespace PhotoCat.Application.Photos.RestorePhoto;

public sealed class RestorePhotoCommand(Guid photoId) : IRequest
{
    public Guid PhotoId { get; set; } = photoId;
}
