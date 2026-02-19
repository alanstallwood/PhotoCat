namespace PhotoCat.Domain.Exceptions
{
    public sealed class InvalidFilePathException : DomainException
    {
        public InvalidFilePathException() : base("FilePath cannot be empty.") { }
    }
}
