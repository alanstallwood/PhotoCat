using PhotoCat.Domain.Photos;

namespace PhotoCat.Infrastructure.Metadata
{
    public interface IMetadataMapper
    {
        static abstract PhotoMetadata Map(IDictionary<string, string> rawExif);
    }
}