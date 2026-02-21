using MediatR;

namespace PhotoCat.Application.Photos.RestorePhotoFile;

public sealed class RestorePhotoFileCommand(Guid photoId, Guid photoFileId) : IRequest
{
    public Guid PhotoId { get; set; } = photoId;
    public Guid PhotoFileId { get; set; } = photoFileId;
}
