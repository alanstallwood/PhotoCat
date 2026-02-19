using MediatR;
using PhotoCat.Infrastructure.Photos;

namespace PhotoCat.Application.Photos.AddPhoto
{
    public sealed class AddPhotoCommand : IRequest<AddPhotoResult>
    {
        public IReadOnlyCollection<string>? FilePaths { get; init; } = [];
        public IEnumerable<string>? Tags { get; init; } = [];
    }

}
