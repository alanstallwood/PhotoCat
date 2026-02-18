namespace PhotoCat.Domain.Photos
{
    public class PhotoDeletionException : Exception
    {
        public Guid PhotoId { get; }

        public PhotoDeletionException(Guid photoId)
            : base($"Cannot delete photo {photoId} with active files.")
        {
            PhotoId = photoId;
        }
    }
}
