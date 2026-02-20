using MediatR;

namespace PhotoCat.Application.Photos.AddPhotoFile
{
    public sealed class AddPhotoFileCommand : IRequest<Guid>
    {
        public Guid PhotoId { get; set; }
        public Stream File { get; init; } = null!;
        public string FileName { get; init; } = null!;
    }
}
