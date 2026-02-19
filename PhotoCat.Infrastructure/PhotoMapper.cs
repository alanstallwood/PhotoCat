using NetTopologySuite.Geometries;
using PhotoCat.Domain.Photos;
using PhotoCat.Infrastructure.Persistence.Enities;
using System.Text.Json;

namespace PhotoCat.Infrastructure
{
    public static class PhotoMapper
    {
        public static PhotoRecord ToRecord(Photo photo)
        {
            return new PhotoRecord
            {
                Id = photo.Id,
                DateTaken = photo.DateTaken,
                CameraMake = photo.Camera?.Make,
                CameraModel = photo.Camera?.Model,
                CameraLens = photo.Camera?.Lens,
                ExposureIso = photo.Exposure?.Iso,
                ExposureFNumber = photo.Exposure?.FNumber,
                ExposureTime = photo.Exposure?.Time,
                ExposureFocalLength = photo.Exposure?.FocalLength,
                Location = photo.Location != null && photo.Location.Longitude.HasValue && photo.Location.Latitude.HasValue ? new Point(photo.Location.Longitude.Value, photo.Location.Latitude.Value) : null,
                Altitude = photo.Location?.Altitude,
                RawExifJson = photo.RawExif != null ? JsonSerializer.Serialize(photo.RawExif) : null,
                Files = [.. photo.Files.Select(f => new PhotoFileRecord
            {
                Id = f.Id,
                PhotoId = f.PhotoId,
                FileName = f.FileName,
                FilePath = f.FilePath,
                FileType = f.FileType,
                Width = f.Dimensions?.Width,
                Height = f.Dimensions?.Height,
                Orientation = f.Dimensions?.Orientation,
                SizeBytes = f.SizeBytes,
                Checksum = f.Checksum,
                Notes = f.Notes,
                IsDeleted = f.IsDeleted,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            })],
                Tags = photo.Tags,
                IsDeleted = photo.IsDeleted,
                CreatedAt = photo.CreatedAt,
                UpdatedAt = photo.UpdatedAt
            };

        }

        public static Photo ToDomain(PhotoRecord record)
        {

            var rawExif = string.IsNullOrEmpty(record.RawExifJson)
                ? null
                : JsonSerializer.Deserialize<Dictionary<string, string>>(record.RawExifJson);


            CameraInfo? camera = BuildCamera(record);
            ExposureInfo? exposure = BuildExposure(record);
            GeoLocation? location = BuildGeoLocation(record);

            return new Photo(
                record.Id,
                record.DateTaken,
                camera,
                exposure,
                location,
                rawExif,
                record.Files.Select(f => new PhotoFile(
                    f.Id,
                    f.PhotoId,
                    f.FileName,
                    f.FilePath,
                    f.FileType,
                    f.SizeBytes,
                    f.Checksum,
                    BuildDimensions(f),
                    f.Notes,
                    f.IsDeleted,
                    f.CreatedAt,
                    f.UpdatedAt
                )),
                record.RepresentativeFileId,
                record.Tags,
                record.IsDeleted,
                record.CreatedAt,
                record.UpdatedAt
            );
        }

        private static CameraInfo? BuildCamera(PhotoRecord record)
        {
            if (string.IsNullOrWhiteSpace(record.CameraMake) &&
                string.IsNullOrWhiteSpace(record.CameraModel) &&
                string.IsNullOrWhiteSpace(record.CameraLens))
            {
                return null;
            }

            return new CameraInfo(
                record.CameraMake,
                record.CameraModel,
                record.CameraLens
            );
        }

        private static ExposureInfo? BuildExposure(PhotoRecord record)
        {
            if (string.IsNullOrWhiteSpace(record.ExposureIso) &&
                !record.ExposureFNumber.HasValue &&
                string.IsNullOrWhiteSpace(record.ExposureTime) &&
                !record.ExposureFocalLength.HasValue)
            {
                return null;
            }

            return new ExposureInfo(
                record.ExposureIso,
                record.ExposureFNumber,
                record.ExposureTime,
                record.ExposureFocalLength
            );
        }

        private static Dimensions? BuildDimensions(PhotoFileRecord record)
        {
            if (!record.Width.HasValue && !record.Height.HasValue && !record.Orientation.HasValue)
                return null;

            return new Dimensions(
                record.Width,
                record.Height,
                record.Orientation
            );
        }

        private static GeoLocation? BuildGeoLocation(PhotoRecord record)
        {
            if (record.Location?.Coordinates is null && !record.Altitude.HasValue)
                return null;

            return new GeoLocation(
                record.Location?.Coordinate?.X,
                record.Location?.Coordinate?.Y,
                record.Altitude
            );
        }
    }
}
