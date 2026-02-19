namespace PhotoCat.Application.Exceptions
{
    public sealed class FileNotFoundApplicationException(string path) 
        : ApplicationExceptionBase($"File not found: {path}")
    {
    }
}
