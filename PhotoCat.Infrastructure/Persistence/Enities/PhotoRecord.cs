using NetTopologySuite.Geometries;
using PhotoCat.Domain.Photos;

namespace PhotoCat.Infrastructure.Persistence.Enities
{
    public class PhotoRecord
    {
        public Guid Id { get; set; }

        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public DateTime? DateTaken { get; set; }

        public string? FileFormat { get; set; } = null!;
        public long? SizeBytes { get; set; }
        public byte[] Checksum { get; set; } = null!;

        public IReadOnlyCollection<Tag> Tags { get; set; } = [];

        // CameraInfo
        public string? CameraMake { get; set; }
        public string? CameraModel { get; set; }
        public string? CameraLens { get; set; }

        // ExposureInfo
        public string? ExposureIso { get; set; }            
        public decimal? ExposureFNumber { get; set; }       
        public string? ExposureTime { get; set; }            
        public decimal? ExposureFocalLength { get; set; }    

        // Dimensions
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? Orientation { get; set; }

        // GeoLocation
        public Point? Location { get; set; }                 
        public double? Altitude { get; set; }
        
        // Raw EXIF dictionary stored as JSONB
        public string? RawExifJson { get; set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
    }
}
