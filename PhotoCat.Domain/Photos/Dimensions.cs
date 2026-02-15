namespace PhotoCat.Domain.Photos
{
    public sealed class Dimensions(int? width, int? height, int? orientation)
    {
        public int? Width { get; set; } = width;
        public int? Height { get; set; } = height;
        public int? Orientation { get; set; } = orientation;
    }
}
