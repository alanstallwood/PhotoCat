using PhotoCat.Domain.Photos;
using PhotoCat.Infrastructure.Photos;

namespace PhotoCat.Application.Photos
{
    public interface IPhotoRepository
    {
        Task<Photo?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<AddPhotoResult> AddAsync(Photo photo, CancellationToken ct);
        Task Update(Photo photo, CancellationToken ct);
        Task<bool> FileChecksumExistsAsync(byte[] checksum, CancellationToken ct);
        Task<Photo?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken ct);
    }
}
