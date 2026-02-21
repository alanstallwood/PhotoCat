namespace PhotoCat.Domain.Exceptions;

public sealed class PhotoFilesMustHaveAPositvieSizeValueException : DomainException
{
    public PhotoFilesMustHaveAPositvieSizeValueException(long size)
        : base($"File size cannot be negative: {size}") { }
}
