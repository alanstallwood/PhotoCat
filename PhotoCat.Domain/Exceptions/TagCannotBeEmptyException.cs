namespace PhotoCat.Domain.Exceptions
{
    public sealed class TagCannotBeEmptyException() 
        : DomainException("Tag cannot be empty")
    {
    }
}
