namespace PhotoCat.Domain.Photos
{
    public sealed class ExposureInfo(string? iso, decimal? aperture, string? shutterSpeed, decimal? focalLength)
    {
        string? Iso { get; set; } = iso;
        decimal? Apertuere { get; set; } = aperture;
        string? ShutterSpeed { get; set; } = shutterSpeed;
        decimal? FocalLength { get; set; } = focalLength;
    }
}
