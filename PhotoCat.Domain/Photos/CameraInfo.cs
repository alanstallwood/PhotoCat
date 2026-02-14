namespace PhotoCat.Domain.Photos
{
    public sealed class CameraInfo(string? make, string? model, string? lens)
    {
        string? Make { get; set; } = make;
        string? Model { get; set; } = model;
        string? Lens { get; set; } = lens;
    }
}
