using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PhotoCat.Infrastructure")]
namespace PhotoCat.Domain.Photos
{
    public sealed class Photo
    {
        public Guid Id { get; private set; }
        public DateTime? DateTaken { get; private set; }

        public CameraInfo? Camera { get; private set; } = null!;
        public ExposureInfo? Exposure { get; private set; }
        public GeoLocation? Location { get; private set; }

        public IReadOnlyDictionary<string, string>? RawExif { get; init; }

        private readonly List<PhotoFile> _files = [];
        public IReadOnlyCollection<PhotoFile> Files => _files.Where(f => !f.IsDeleted).ToList().AsReadOnly();

        public Guid RepresentativeFileId { get; private set; }

        // Tags are a value object collection
        private readonly List<Tag> _tags = [];
        public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

        public bool IsDeleted { get; private set; }

        // DB-managed audit fields
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }


        // EF needs a private constructor
        private Photo() { }

        private Photo(PhotoMetadata? metadata = null)
        {
            Id = Guid.NewGuid();
            DateTaken = metadata?.DateTaken;
            Camera = metadata?.Camera;
            Exposure = metadata?.Exposure;
            Location = metadata?.Location;
            RawExif = metadata?.RawExif;
        }

        public static Photo Create(PhotoMetadata? metadata = null, IEnumerable<string>? tags = null)
        {
            var photo = new Photo(metadata);

            if (tags != null)
            {
                foreach (var tagName in tags)
                    photo.AddTag(tagName);
            }

            return photo;
        }

        /// <summary>
        /// Used for rehydration by repo
        /// </summary>
        internal Photo(
            Guid id,
            DateTime? dateTaken,
            CameraInfo? camera,
            ExposureInfo? exposure,
            GeoLocation? location,
            IReadOnlyDictionary<string, string>? rawExif,
            IEnumerable<PhotoFile>? files,
            Guid representativeFileId,
            IEnumerable<Tag>? tags,
            bool isDeleted,
            DateTime createdAt,
            DateTime updatedAt)
        {
            Id = id;
            DateTaken = dateTaken;
            Camera = camera;
            Exposure = exposure;
            Location = location;
            RawExif = rawExif;

            if (files != null)
                _files.AddRange(files);

            RepresentativeFileId = representativeFileId;

            if (tags != null)
                _tags.AddRange(tags);

            IsDeleted = isDeleted;

            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public void AddFile(string fileName, string filePath, PhotoFileType fileType, long? sizeBytes, byte[] checksum, PhotoMetadata? metadata = null)
        {
            var photoFile = PhotoFile.Create(fileName, filePath, fileType, sizeBytes, checksum, metadata);
            _files.Add(photoFile);
        }

        public void SoftDelete()
        {
            if (Files.Count != 0)
            {
                throw new PhotoDeletionException(Id);
            }
            IsDeleted = true;
        }

        public void Restore()
        {
            IsDeleted = false;
        }

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

        public void SetRepresentativeFile(Guid newRepresentativeFileId)
        {
            var file = _files.SingleOrDefault(f => f.Id == newRepresentativeFileId);
            if(file is null)
            {
                return;
            }
            RepresentativeFileId = newRepresentativeFileId;
        }

        public void SoftDeleteFile(Guid photoFileId)
        {
            var file = _files.SingleOrDefault(f => f.Id == photoFileId);
            file?.SoftDelete();
        }

        public void RestoreFile(Guid photoFileId)
        {
            var file = _files.SingleOrDefault(f => f.Id == photoFileId);
            file?.Restore();

        }
    }
}
