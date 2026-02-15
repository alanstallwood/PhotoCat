namespace PhotoCat.Domain.Photos
{
    public sealed class ExposureInfo(string? iso, decimal? aperture, string? shutterSpeed, decimal? focalLength)
    {
        public string? Iso { get; set; } = iso;
        public decimal? FNumber { get; set; } = aperture;
        public string? Time { get; set; } = shutterSpeed;
        public decimal? FocalLength { get; set; } = focalLength;
    }
}
