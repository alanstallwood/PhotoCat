using MediatR;

namespace PhotoCat.Application.Photos.AddPhotoFileFromPath;

public sealed class AddPhotoFileFromPathCommand : IRequest<Guid>
{
    public Guid PhotoId { get; set; }
    public string FilePath { get; init; } = null!;
    public string FileName { get; init; } = null!;
}
