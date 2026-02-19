namespace PhotoCat.Domain.Exceptions
{
    public sealed class InvalidChecksumException : DomainException
    {
        public InvalidChecksumException() : base("Checksum cannot be empty.") { }
    }
}
