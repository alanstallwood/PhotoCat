namespace PhotoCat.Domain.Photos
{
    public sealed class Dimensions(int? width, int? height, int? orientation)
    {
        int? Width { get; set; } = width;
        int? Height { get; set; } = height;
        int? Orientation { get; set; } = orientation;
    }
}
