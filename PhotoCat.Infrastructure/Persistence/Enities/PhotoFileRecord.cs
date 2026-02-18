using PhotoCat.Domain.Photos;

namespace PhotoCat.Infrastructure.Persistence.Enities
{
    public class PhotoFileRecord
    {
        public Guid Id { get; set; }
        public Guid PhotoId { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public PhotoFileType FileType { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? Orientation { get; set; }

        public long? SizeBytes { get; set; }
        public byte[] Checksum { get; set; } = null!;
        public string? Notes { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}