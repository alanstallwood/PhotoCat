namespace PhotoCat.Domain.Exceptions;

public sealed class PhotoFileMustHaveAFileNameException : DomainException
{
    public PhotoFileMustHaveAFileNameException() : base("FileName cannot be empty.") { }
}
