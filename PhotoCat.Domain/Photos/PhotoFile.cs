using PhotoCat.Domain.Exceptions;

namespace PhotoCat.Domain.Photos
{
    public class PhotoFile
    {
        public Guid Id { get; private set; }
        public Guid PhotoId { get; private set; }
        public string FileName { get; private set; } = null!;
        public string FilePath { get; private set; } = null!;
        public PhotoFileType FileType { get; private set; }
        public Dimensions? Dimensions { get; private set; }
        public long? SizeBytes { get; private set; }
        public byte[] Checksum { get; private set; } = null!;
        public string? Notes { get; private set; }
        // TODO: Do we need a role Original/Edit/Export/Thumbnail...
        public bool IsDeleted { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private PhotoFile() { }

        private PhotoFile(string fileName, string filePath, PhotoFileType fileType, long? sizeBytes, byte[] checksum, PhotoMetadata? metadata = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new InvalidFileNameException();

            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidFilePathException();

            if (checksum is null || checksum.Length == 0)
                throw new InvalidChecksumException();

            if (sizeBytes < 0)
                throw new InvalidFileSizeException(sizeBytes.Value);


            Id = Guid.NewGuid();
            FileName = fileName;
            FilePath = filePath;
            FileType = fileType;
            SizeBytes = sizeBytes;
            Checksum = checksum;
            Dimensions = metadata?.Dimensions;
        }

        public static PhotoFile Create(string fileName, string filePath, PhotoFileType fileType, long? sizeBytes, byte[] checksum, PhotoMetadata? metadata = null)
        {
            return new PhotoFile(fileName, filePath, fileType, sizeBytes, checksum, metadata);
        }

        internal PhotoFile(Guid id, Guid photoId, string fileName, string filePath, PhotoFileType fileType, long? sizeBytes, byte[] checksum, Dimensions? dimensions, string? notes, bool isDeleted, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            PhotoId = photoId;
            FileName = fileName;
            FilePath = filePath;
            FileType = fileType;
            SizeBytes = sizeBytes;
            Checksum = checksum;
            Dimensions = dimensions;
            Notes = notes;
            IsDeleted = isDeleted;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public void ReplaceFile(PhotoFileType fileType, long? sizeBytes, byte[] checksum, PhotoMetadata? metadata = null)
        {
            FileType = fileType;
            SizeBytes = sizeBytes;
            Checksum = checksum;
            Dimensions = metadata?.Dimensions;
        }

        public void Rename(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
        }

        public void Restore()
        {
            IsDeleted = false;
        }

        public void AmendNotes(string notes)
        {
            Notes = notes;
        }
    }
}
