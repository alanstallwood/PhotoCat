namespace PhotoCat.Domain.Photos
{
    public sealed class CameraInfo(string? make, string? model, string? lens)
    {
        public string? Make { get; set; } = make;
        public string? Model { get; set; } = model;
        public string? Lens { get; set; } = lens;
    }
}
