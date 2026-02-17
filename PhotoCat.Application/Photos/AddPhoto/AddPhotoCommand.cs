namespace PhotoCat.Application.Photos.AddPhoto
{
    public sealed class AddPhotoCommand(
        string filePath,
        IEnumerable<string>? tags = null)
    {
        public string FilePath { get; init; } = filePath;
        public IEnumerable<string>? Tags { get; init; } = tags;
    }

}
