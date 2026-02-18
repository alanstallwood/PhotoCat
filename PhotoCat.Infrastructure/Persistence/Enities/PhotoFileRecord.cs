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
        public int? DimensionWidth { get; set; }
        public int? DimensionHeight { get; set; }
        public int? DimensionOrientation { get; set; }

        public long? SizeBytes { get; set; }
        public byte[] Checksum { get; set; } = null!;
        public string? Notes { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}