namespace PhotoCat.Domain.Exceptions;

public sealed class PhotoFilesMustHaveAChecksumException : DomainException
{
    public PhotoFilesMustHaveAChecksumException() : base("Checksum cannot be empty.") { }
}
