using PhotoCat.Domain.Photos;

namespace PhotoCat.Application.Photos
{
    public interface IPhotoRepository
    {
        Task<Photo?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Guid> AddAsync(Photo photo, CancellationToken ct);
        Task UpdateAsync(Photo photo, CancellationToken ct);
        Task<bool> FileChecksumExistsAsync(byte[] checksum, CancellationToken ct = default);
        Task<Photo?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken ct);
        Task<Photo?> GetByFileGroupName(string fileGroupName, CancellationToken ct);
    }
}
