using MediatR;

namespace PhotoCat.Application.Photos.AddPhotoFile
{
    public sealed class AddPhotoFileCommand(Guid photoId, Stream file, string fileName) : IRequest<Guid>
    {
        public Guid PhotoId { get; set; } = photoId;
        public Stream File { get; init; } = file;
        public string FileName { get; init; } = fileName;
    }
}
