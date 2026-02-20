namespace PhotoCat.Application.Exceptions;

public sealed class FileAlreadyExistsException()
    : ApplicationExceptionBase("A file with the same checksum already exists.")
{
}
