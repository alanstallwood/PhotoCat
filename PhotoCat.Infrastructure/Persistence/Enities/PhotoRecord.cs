using NetTopologySuite.Geometries;
using PhotoCat.Domain.Photos;

namespace PhotoCat.Infrastructure.Persistence.Enities
{
    public class PhotoRecord
    {
        public Guid Id { get; set; }
        public DateTime? DateTaken { get; set; }


        // CameraInfo
        public string? CameraMake { get; set; }
        public string? CameraModel { get; set; }
        public string? CameraLens { get; set; }

        // ExposureInfo
        public string? ExposureIso { get; set; }            
        public decimal? ExposureFNumber { get; set; }       
        public string? ExposureTime { get; set; }            
        public decimal? ExposureFocalLength { get; set; }    

        // GeoLocation
        public Point? Location { get; set; }                 
        public double? Altitude { get; set; }

        public string? RawExifJson { get; set; }
        public IReadOnlyCollection<PhotoFileRecord> Files { get; set; } = [];
        public IReadOnlyCollection<Tag> Tags { get; set; } = [];

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
