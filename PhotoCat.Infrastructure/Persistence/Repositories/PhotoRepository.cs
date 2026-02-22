using Microsoft.EntityFrameworkCore;
using PhotoCat.Application.Photos;
using PhotoCat.Domain.Photos;


namespace PhotoCat.Infrastructure.Photos;


public sealed class PhotoRepository(PhotoCatDbContext db) : IPhotoRepository
{
    private readonly PhotoCatDbContext _db = db;

    public async Task<Photo?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var record = await _db.Photos
            .Include(p => p.Tags)
            .Include(p => p.Files)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        return record != null ? PhotoMapper.ToDomain(record) : null;
    }

    public async Task<Photo?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken ct)
    {
        var record = await _db.Photos
            .IgnoreQueryFilters() // Include soft-deleted records
            .Include(p => p.Tags)
            .Include(p => p.Files)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        return record != null ? PhotoMapper.ToDomain(record) : null;
    }

    public async Task<Guid> AddAsync(
        Photo photo,
        CancellationToken ct)
    {
        var record = PhotoMapper.ToRecord(photo);
        _db.Photos.Add(record);

        await _db.SaveChangesAsync(ct);

        return photo.Id;
    }

    public async Task<bool> FileChecksumExistsAsync(byte[] checksum, CancellationToken ct = default)
    {
        return await _db.Photos
            .SelectMany(p => p.Files)
            .Where(f => f.Checksum.SequenceEqual(checksum) && !f.IsDeleted)
            .AnyAsync(ct);
    }

    public async Task UpdateAsync(Photo photo, CancellationToken ct)
    {
        var record = PhotoMapper.ToRecord(photo);
        _db.Photos.Update(record);

        await _db.SaveChangesAsync(ct);
    }

    public async Task<Photo?> GetByGroupKeyAsync(string groupKey, CancellationToken ct)
    {
        var record = await _db.Photos
            .FirstOrDefaultAsync(p => p.GroupKey == groupKey && !p.IsDeleted, ct);

        return record != null ? PhotoMapper.ToDomain(record) : null;
    }

    public async Task<IEnumerable<PhotoFileFullPathAndIdsDto>> GetAllPhotoFileFullPathsAsync(CancellationToken ct = default)
    {
        var fileProperties = await _db.Photos
            .SelectMany(p => p.Files)
            .Select(f => new { f.Id, f.PhotoId, f.FilePath, f.FileName })
            .ToListAsync(ct);

        return fileProperties.Select(f => new PhotoFileFullPathAndIdsDto(f.PhotoId, f.Id, Path.Combine(f.FilePath, f.FileName)));
    }
}