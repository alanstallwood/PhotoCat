namespace PhotoCat.Application.Photos.AddPhoto
{
    public sealed class AddPhotoCommand
    {
        public string FileName { get; init; }
        public string FilePath { get; init; }
        public string FileFormat { get; init; }
        public long SizeBytes { get; init; }
        public string Checksum { get; init; }
        public DateTime? DateTaken { get; init; }
        public IEnumerable<string>? Tags { get; init; }

        public AddPhotoCommand(
            string fileName,
            string filePath,
            string fileFormat,
            long sizeBytes,
            string checksum,
            DateTime? dateTaken = null,
            IEnumerable<string>? tags = null)
        {
            FileName = fileName;
            FilePath = filePath;
            FileFormat = fileFormat;
            SizeBytes = sizeBytes;
            Checksum = checksum;
            DateTaken = dateTaken;
            Tags = tags;
        }
    }

}
