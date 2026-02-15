using Microsoft.EntityFrameworkCore;
using Npgsql;
using PhotoCat.Application.Photos;
using PhotoCat.Domain.Photos;


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

        photo.Id = dbResult.Id;

        return new AddPhotoResult(dbResult.Id, dbResult.Inserted, photo);
    }

}