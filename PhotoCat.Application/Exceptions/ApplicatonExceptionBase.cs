namespace PhotoCat.Application.Exceptions;

public abstract class ApplicationExceptionBase(string message) 
    : Exception(message)
{
}
