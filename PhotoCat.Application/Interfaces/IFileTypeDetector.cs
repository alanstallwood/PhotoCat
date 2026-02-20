using PhotoCat.Domain.Photos;

namespace PhotoCat.Application.Interfaces
{
    public interface IFileTypeDetector
    {
        PhotoFileType Detect(string filePath);
        PhotoFileType Detect(Stream stream);
    }
}
