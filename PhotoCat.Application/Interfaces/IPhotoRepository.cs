using PhotoCat.Domain.Photos;

namespace PhotoCat.Application.Photos
{
    public interface IPhotoRepository
    {
        Task<Photo?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Guid> AddAsync(Photo photo, CancellationToken ct = default);
        Task UpdateAsync(Photo photo, CancellationToken ct = default);
        Task<bool> FileChecksumExistsAsync(byte[] checksum, CancellationToken ct = default);
        Task<Photo?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken ct = default);
        Task<Photo?> GetByGroupKeyAsync(string groupKey, CancellationToken ct = default);
        Task<IEnumerable<PhotoFileFullPathAndIdsDto>> GetAllPhotoFileFullPathsAsync(CancellationToken ct = default);
    }
}
