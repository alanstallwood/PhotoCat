using Microsoft.EntityFrameworkCore;
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


    public async Task AddAsync(Photo photo, CancellationToken ct)
    {
        _db.Photos.Add(photo);
    }
}