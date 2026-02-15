using PhotoCat.Domain.Photos;

namespace PhotoCat.Application.Photos.AddPhoto
{
    public record AddPhotoResult(
        Guid Id,
        bool Inserted,
        Photo Photo);
}
  