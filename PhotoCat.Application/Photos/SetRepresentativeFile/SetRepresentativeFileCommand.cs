using MediatR;

namespace PhotoCat.Application.Photos.SetRepresentativeFile;

public sealed class SetRepresentativeFileCommand(Guid photoId, Guid photoFileId) : IRequest
{
    public Guid PhotoId { get; set; } = photoId;
    public Guid PhotoFileId { get; set; } = photoFileId;
}
