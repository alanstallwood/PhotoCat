using PhotoCat.Domain.Photos;

namespace PhotoCat.Infrastructure.Photos
{
    public sealed class AddPhotoResult
    {
        public Guid Id { get; }
        public bool IsCreated { get; }

        private AddPhotoResult(Guid id, bool isCreated)
        {
            Id = id;
            IsCreated = isCreated;
        }

        public static AddPhotoResult Created(Guid id)
            => new(id, true);

        public static AddPhotoResult AlreadyExists(Guid id)
            => new(id, false);
    }
}
  