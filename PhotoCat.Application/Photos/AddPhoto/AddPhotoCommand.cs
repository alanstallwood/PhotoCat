using MediatR;

namespace PhotoCat.Application.Photos.AddPhoto
{
    public sealed class AddPhotoCommand(IReadOnlyCollection<string>? fullFilePaths, IEnumerable<string>? tags = default) : IRequest<Guid>
    {
        public IReadOnlyCollection<string>? FullFilePaths { get; init; } = fullFilePaths;
        public IEnumerable<string>? Tags { get; init; } = tags;
    }

}
