using PhotoCat.Domain.Photos;

namespace PhotoCat.Infrastructure.Metadata
{
    public interface IExifExtractor
    {
        PhotoMetadata? Extract(string filePath);
        PhotoMetadata? Extract(Stream fileStream, string fileName);
    }
}