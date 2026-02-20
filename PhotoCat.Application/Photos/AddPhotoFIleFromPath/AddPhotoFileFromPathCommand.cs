using MediatR;

namespace PhotoCat.Application.Photos.AddPhotoFileFromPath;

public sealed class AddPhotoFileFromPathCommand(Guid photoId, string filePath, string fileName) : IRequest<Guid>
{
    public Guid PhotoId { get; set; } = photoId;
    public string FilePath { get; init; } = filePath;
    public string FileName { get; init; } = fileName;
}
