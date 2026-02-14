namespace PhotoCat.Domain.Photos
{
    public sealed class PhotoMetadata
    {
        public DateTime? DateTaken { get; init; }
        public CameraInfo? Camera { get; init; }
        public ExposureInfo? Exposure { get; init; }
        public Dimensions? Dimensions { get; init; }
        public GeoLocation? Location { get; init; }
        public IReadOnlyDictionary<string, string>? RawExif { get; init; } 
    }
}
