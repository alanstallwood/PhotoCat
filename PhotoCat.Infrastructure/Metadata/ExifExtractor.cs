using MetadataExtractor;
using PhotoCat.Domain.Photos;

namespace PhotoCat.Infrastructure.Metadata;

public sealed class ExifExtractor : IExifExtractor
{
    public PhotoMetadata? Extract(string filePath)
    {
        var directories = ImageMetadataReader.ReadMetadata(filePath);
        return BuildPhotoMetaData(directories);
    }

    public PhotoMetadata? Extract(Stream fileStream, string fileName)
    {
        var directories = ImageMetadataReader.ReadMetadata(fileStream, fileName);

        return BuildPhotoMetaData(directories);
    }


    private static PhotoMetadata? BuildPhotoMetaData(IReadOnlyList<MetadataExtractor.Directory> directories)
    {
        try
        {
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
        catch (Exception)
        {
            return null;
            //TODO: Probably should log this exception
        }
    }
}