using PhotoCat.Domain.Photos;

namespace PhotoCat.Application.Photos
{
    public interface IPhotoRepository
    {
        Task<Photo?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Guid> AddAsync(Photo photo, CancellationToken ct);
        Task UpdateAsync(Photo photo, CancellationToken ct);
        Task<bool> FileChecksumExistsAsync(byte[] checksum, CancellationToken ct);
        Task<Photo?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken ct);
    }
}
