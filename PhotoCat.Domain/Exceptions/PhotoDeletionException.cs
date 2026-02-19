namespace PhotoCat.Domain.Exceptions
{
    public sealed class PhotoDeletionException(Guid photoId) 
        : DomainException($"Cannot delete photo {photoId} with active files.")
    {
        public Guid PhotoId { get; } = photoId;
    }
}
