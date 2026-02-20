namespace PhotoCat.Application.Exceptions
{
    public class PhotoNotFoundException(Guid id) 
        : ApplicationExceptionBase($"Photo with ID {id} not found.")
    {
    }
}
