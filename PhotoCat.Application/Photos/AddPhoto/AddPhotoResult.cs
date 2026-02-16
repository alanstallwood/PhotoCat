using PhotoCat.Domain.Photos;

namespace PhotoCat.Infrastructure.Photos
{
    public record AddPhotoResult(
        Guid Id,
        bool Inserted);
}
  