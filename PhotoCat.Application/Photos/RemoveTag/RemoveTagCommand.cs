using MediatR;

namespace PhotoCat.Application.Photos.RemoveTag;

public sealed class RemoveTagCommand : IRequest
{
    public Guid PhotoId { get; set; }
    public string Tag { get; set; } = null!;
}
