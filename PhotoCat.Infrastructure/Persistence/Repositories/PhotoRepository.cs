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


    public async Task<AddPhotoResult> InsertOrGetPhotoAsync(
    Photo photo,
    CancellationToken ct)
    {
        var record = MapPhotoRecord(photo);
        var sql = @"
        INSERT INTO photos (file_name, file_path, date_taken, file_format, size_bytes, checksum)
        VALUES (@filaname, @filepath, @dateTaken, @fileFormat, @sizeBytes, @checksum)
        ON CONFLICT (checksum)
        DO UPDATE SET checksum = EXCLUDED.checksum
        RETURNING id, (xmax = 0) AS inserted;
    ";

        var dbResult = await _db.Database
            .SqlQueryRaw<AddPhotoResult>(sql,
                new NpgsqlParameter("filaname", photo.FileName),
                new NpgsqlParameter("filepath", photo.FilePath),
                new NpgsqlParameter("dateTaken", photo.DateTaken ?? (object)DBNull.Value),
                new NpgsqlParameter("fileFormat", photo.FileFormat ?? (object)DBNull.Value),
                new NpgsqlParameter("sizeBytes", photo.SizeBytes ?? (object)DBNull.Value),
                new NpgsqlParameter("checksum", photo.Checksum)                )
            .SingleAsync(ct);


        return new AddPhotoResult(dbResult.Id, dbResult.Inserted, photo);
    }

    private PhotoRecord MapPhotoRecord(Photo photo)
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