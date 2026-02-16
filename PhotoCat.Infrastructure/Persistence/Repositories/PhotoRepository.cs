using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Npgsql;
using PhotoCat.Application.Photos;
using PhotoCat.Domain.Photos;
using PhotoCat.Infrastructure.Persistence.Enities;


namespace PhotoCat.Infrastructure.Photos;


public sealed class PhotoRepository : IPhotoRepository
{
    private readonly PhotoCatDbContext _db;


    public PhotoRepository(PhotoCatDbContext db)
    {
        _db = db;
    }


    public Task<Photo?> GetByIdAsync(Guid id, CancellationToken ct)
    => _db.Photos
    .Include(p => p.Tags)
    .FirstOrDefaultAsync(p => p.Id == id, ct);


    public async Task<AddPhotoResult> AddIfNotExistsAsync(
    Photo photo,
    CancellationToken ct)
    {
        var record = MapPhotoRecord(photo);
        var sql = @"
        INSERT INTO photos (id, file_name, file_path, date_taken, file_format, size_bytes, checksum, camera_make, camera_model,
        camera_lens, exposure_iso, exposure_fnumber, exposure_time, exposure_focallength, width, height, orientation, location, 
        altitude, raw_exif)
        VALUES (@id, @filaname, @filepath, @dateTaken, @fileFormat, @sizeBytes, @checksum, @cameraMake, @cameraModel,
        @cameraLens, @exposureIso, @exposureFnumber, @exposureTime, @exposureFocallength, @width, @height, @orientation, @location,
        @altiude, @rawExif)
        ON CONFLICT (checksum)
        DO UPDATE SET checksum = EXCLUDED.checksum
        RETURNING id, (xmax = 0) AS inserted;
    ";

        var dbResult = await _db.Database
            .SqlQueryRaw<AddPhotoResult>(sql,
                new NpgsqlParameter("id", record.Id),
                new NpgsqlParameter("filaname", record.FileName),
                new NpgsqlParameter("filepath", record.FilePath),
                new NpgsqlParameter("dateTaken", record.DateTaken ?? (object)DBNull.Value),
                new NpgsqlParameter("fileFormat", record.FileFormat ?? (object)DBNull.Value),
                new NpgsqlParameter("sizeBytes", record.SizeBytes ?? (object)DBNull.Value),
                new NpgsqlParameter("checksum", record.Checksum),
                new NpgsqlParameter("cameraMake", record.CameraMake ?? (object)DBNull.Value),
                new NpgsqlParameter("cameraModel", record.CameraModel ?? (object)DBNull.Value),
                new NpgsqlParameter("cameraLens", record.CameraLens ?? (object)DBNull.Value),
                new NpgsqlParameter("exposureIso", record.ExposureIso ?? (object)DBNull.Value),
                new NpgsqlParameter("exposureFnumber", record.ExposureFNumber ?? (object)DBNull.Value),
                new NpgsqlParameter("exposureTime", record.ExposureTime ?? (object)DBNull.Value),
                new NpgsqlParameter("exposureFocallength", record.ExposureFocalLength ?? (object)DBNull.Value),
                new NpgsqlParameter("width", record.Width ?? (object)DBNull.Value),
                new NpgsqlParameter("height", record.Height ?? (object)DBNull.Value),
                new NpgsqlParameter("orientation", record.Orientation ?? (object)DBNull.Value),
                new NpgsqlParameter("location", record.Location ?? (object)DBNull.Value),
                new NpgsqlParameter("altiude", record.Altitude ?? (object)DBNull.Value),
                new NpgsqlParameter("rawExif", record.RawExifJson ?? (object)DBNull.Value)
                )
            .SingleAsync(ct);

        return new AddPhotoResult(dbResult.Id, dbResult.Inserted);
    }

    private static PhotoRecord MapPhotoRecord(Photo photo)
    {
        return new PhotoRecord
        {
            Id = photo.Id,
            FileName = photo.FileName,
            FilePath = photo.FilePath,
            DateTaken = photo.DateTaken,
            FileFormat = photo.FileFormat,
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
            RawExifJson = photo.RawExif != null ? System.Text.Json.JsonSerializer.Serialize(photo.RawExif) : null
        };        
            
    }
}