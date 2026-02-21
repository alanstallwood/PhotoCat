namespace PhotoCat.Domain.Exceptions;

public sealed class PhotoFilesMustBeUniqueException()
    : DomainException("A file with the same checksum already exists in this photo.")
{
}
