namespace PhotoCat.Domain.Exceptions;

public sealed class PhotoFileMustHaveAFilePathException : DomainException
{
    public PhotoFileMustHaveAFilePathException() : base("FilePath cannot be empty.") { }
}
