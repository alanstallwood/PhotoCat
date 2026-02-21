using MediatR;

namespace PhotoCat.Application.Photos.AddTag;

public sealed  class AddTagCommand : IRequest
{
    public Guid PhotoId { get; set; }
    public string Tag { get; set; } = null!;
}
