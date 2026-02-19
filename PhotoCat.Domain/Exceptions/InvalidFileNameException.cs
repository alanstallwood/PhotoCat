namespace PhotoCat.Domain.Exceptions
{
    public sealed class InvalidFileNameException : DomainException
    {
        public InvalidFileNameException() : base("FileName cannot be empty.") { }
    }
}
