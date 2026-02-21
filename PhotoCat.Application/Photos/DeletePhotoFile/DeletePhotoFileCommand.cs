using MediatR;

namespace PhotoCat.Application.Photos.DeletePhotoFile;

public sealed class DeletePhotoFileCommand(Guid photoId, Guid photoFileId) : IRequest
{
    public Guid PhotoId { get; set; } = photoId;
    public Guid PhotoFileId { get; set; } = photoFileId;
}
