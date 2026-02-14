using MetadataExtractor;
using PhotoCat.Domain.Photos;

namespace PhotoCat.Infrastructure.Metadata
{
    public sealed class ExifExtractor : IExifExtractor
    {
        public PhotoMetadata? Extract(string filePath)
        {
            try
            {
                return ExtractInternal(filePath);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static PhotoMetadata? ExtractInternal(string filePath)
        {
            var directories = ImageMetadataReader.ReadMetadata(filePath);
            var exifData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var directory in directories)
            {
                foreach (var tag in directory.Tags)
                {
                    var key = $"{directory.Name}:{tag.Name}";
                    var value = tag.Description;

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        exifData[key] = value;
                    }
                }
            }
            return MetadataMapper.Map(exifData);
        }
    }
}
