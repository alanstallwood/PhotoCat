namespace PhotoCat.Domain.Photos
{
    public sealed class Photo
    {
        public Guid Id { get; private set; }
        public string FileName { get; private set; } = null!;
        public string FilePath { get; private set; } = null!;
        public DateTime? DateTaken { get; private set; }
        public string FileFormat { get; private set; } = null!;
        public long SizeBytes { get; private set; }
        public string Checksum { get; private set; } = null!;

        public CameraInfo? Camera { get; private set; } = null!;
        public ExposureInfo? Exposure { get; private set; }
        public Dimensions? Dimensions { get; private set; }
        public GeoLocation? Location { get; private set; }

        public IReadOnlyDictionary<string, string>? RawExif { get; init; }

        // Tags are a value object collection
        private readonly List<Tag> _tags = [];
        public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();



        // DB-managed audit fields
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }


        // EF needs a private constructor
        private Photo() { }

        private Photo(string fileName, string filePath, string fileFormat, long sizeBytes, string checksum, PhotoMetadata? metadata = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("FileName cannot be empty.", nameof(fileName));

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("FilePath cannot be empty.", nameof(filePath));

            if (string.IsNullOrWhiteSpace(fileFormat))
                throw new ArgumentException("FileFormat cannot be empty.", nameof(fileFormat));

            if (string.IsNullOrWhiteSpace(checksum))
                throw new ArgumentException("Checksum cannot be empty.", nameof(checksum));

            if (sizeBytes < 0)
                throw new ArgumentOutOfRangeException(nameof(sizeBytes), "SizeBytes cannot be negative.");


            Id = Guid.NewGuid();
            FileName = fileName;
            FilePath = filePath;
            FileFormat = fileFormat;
            SizeBytes = sizeBytes;
            Checksum = checksum;
            DateTaken = metadata?.DateTaken;
            Camera = metadata?.Camera;
            Exposure = metadata?.Exposure;
            Dimensions = metadata?.Dimensions;
            Location = metadata?.Location;
            RawExif = metadata?.RawExif;
        }

        public static Photo Create(string fileName, string filePath, string fileFormat, long sizeBytes, string checksum, IEnumerable<string>? tags = null, PhotoMetadata? metadata = null)
        {
            var photo = new Photo(fileName, filePath, fileFormat, sizeBytes, checksum, metadata);

            if (tags != null)
            {
                foreach (var tagName in tags)
                    photo.AddTag(tagName);
            }

            return photo;
        }

        // Behaviour: add a tag
        public void AddTag(string tagName)
        {
            var tag = new Tag(tagName);

            if (_tags.Any(t => t.Name == tag.Name))
                return; // ignore duplicates within this photo

            _tags.Add(tag);
        }

        public void RemoveTag(string tagName)
        {
            var normalized = tagName.Trim().ToLowerInvariant();
            var existing = _tags.FirstOrDefault(t => t.Name == normalized);
            if (existing != null)
            {
                _tags.Remove(existing);
            }
        }
    }
}
