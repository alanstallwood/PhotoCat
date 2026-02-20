namespace PhotoCat.Application.Exceptions
{
    public sealed class PhotoNotFoundException(Guid id) 
        : ApplicationExceptionBase($"Photo with ID {id} not found.")
    {
    }
}
