namespace PhotoCat.Domain.Exceptions
{
    public sealed class InvalidFileSizeException : DomainException
    {
        public InvalidFileSizeException(long size)
            : base($"File size cannot be negative: {size}") { }
    }
}
