namespace PhotoCat.Application.Exceptions;

public sealed class NoFilesProvidedException() 
    : ApplicationExceptionBase("A photo must have at least one file.")
{        
}

