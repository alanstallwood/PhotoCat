using MediatR;

namespace PhotoCat.Application.Photos.AddPhoto
{
    public sealed class AddPhotoCommand : IRequest<Guid>
    {
        public IReadOnlyCollection<string>? FilePaths { get; init; } = [];
        public IEnumerable<string>? Tags { get; init; } = [];
    }

}
