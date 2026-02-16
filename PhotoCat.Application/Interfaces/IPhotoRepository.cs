using PhotoCat.Domain.Photos;
using PhotoCat.Infrastructure.Photos;

namespace PhotoCat.Application.Photos
{
    public interface IPhotoRepository
    {
        Task<Photo?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<AddPhotoResult> AddIfNotExistsAsync(Photo photo, CancellationToken ct);
    }
}
