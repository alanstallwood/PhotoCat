using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Npgsql;
using PhotoCat.Application.Photos;
using PhotoCat.Domain.Photos;
using PhotoCat.Infrastructure.Persistence.Enities;
using System.Text.Json;
using GeoLocation = PhotoCat.Domain.Photos.GeoLocation;


namespace PhotoCat.Infrastructure.Photos;


public sealed class PhotoRepository(PhotoCatDbContext db) : IPhotoRepository
{
    private readonly PhotoCatDbContext _db = db;

    public async Task<Photo?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var record = await _db.Photos
        .Include(p => p.Tags)
        .FirstOrDefaultAsync(p => p.Id == id, ct);

        return record != null ? ToDomain(record) : null;
    }

    public async Task<AddPhotoResult> AddAsync(
        Photo photo,
        CancellationToken ct)
    {
        var record = ToRecord(photo);
        _db.Photos.Add(record);

        try
        {
            await _db.SaveChangesAsync(ct);

            return AddPhotoResult.Created(photo.Id);
        }
        catch (DbUpdateException ex)
        {
            // PostgreSQL unique constraint violation
            if (IsUniqueConstraintViolation(ex))
            {
                var existing = await GetByChecksumAsync(
                    photo.Checksum, ct);

                if (existing is not null)
                    return AddPhotoResult.AlreadyExists(existing.Id);
            }

            throw;
        }
    }

    public async Task<Photo?> GetByChecksumAsync(byte[] checksum, CancellationToken ct)
    {
        var record = await _db.Photos.FirstOrDefaultAsync(p => p.Checksum == checksum, ct);
        return record != null ? ToDomain(record) : null;
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        if (ex.InnerException is PostgresException pg)
        {
            // 23505 = unique_violation
            return pg.SqlState == "23505";
        }

        return false;
    }

    private static PhotoRecord ToRecord(Photo photo)
    {
        return new PhotoRecord
        {
            Id = photo.Id,
            FileName = photo.FileName,
            FilePath = photo.FilePath,
            DateTaken = photo.DateTaken,
            FileFormat = photo.FileType,
            SizeBytes = photo.SizeBytes,
            Checksum = photo.Checksum,
            CameraMake = photo.Camera?.Make,
            CameraModel = photo.Camera?.Model,
            CameraLens = photo.Camera?.Lens,
            ExposureIso = photo.Exposure?.Iso,
            ExposureFNumber = photo.Exposure?.FNumber,
            ExposureTime = photo.Exposure?.Time,
            ExposureFocalLength = photo.Exposure?.FocalLength,
            Width = photo.Dimensions?.Width,
            Height = photo.Dimensions?.Height,
            Orientation = photo.Dimensions?.Orientation,
            Location = photo.Location != null && photo.Location.Longitude.HasValue && photo.Location.Latitude.HasValue ? new Point(photo.Location.Longitude.Value, photo.Location.Latitude.Value) : null,
            Altitude = photo.Location?.Altitude,
            RawExifJson = photo.RawExif != null ? JsonSerializer.Serialize(photo.RawExif) : null
        };

    }

    private static Photo ToDomain(PhotoRecord record)
    {

        var rawExif = string.IsNullOrEmpty(record.RawExifJson)
            ? null
            : JsonSerializer.Deserialize<Dictionary<string, string>>(record.RawExifJson);


        CameraInfo? camera = BuildCamera(record);
        ExposureInfo? exposure = BuildExposure(record);
        Dimensions? dimensions = BuildDimensions(record);
        GeoLocation? location = BuildGeoLocation(record);

        return new Photo(
            record.Id,
            record.FileName,
            record.FilePath,
            record.FileFormat,
            record.SizeBytes,
            record.Checksum,
            record.DateTaken,
            camera,
            exposure,
            dimensions,
            location,
            rawExif,
            record.Tags,
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

    private static Dimensions? BuildDimensions(PhotoRecord record)
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