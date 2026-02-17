using PhotoCat.Domain.Photos;
using PhotoCat.Infrastructure.Photos;

namespace PhotoCat.Application.Photos
{
    public interface IPhotoRepository
    {
        Task<Photo?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Photo?> GetByChecksumAsync(byte[] checksum, CancellationToken ct);
        Task<AddPhotoResult> AddAsync(Photo photo, CancellationToken ct);
    }
}
