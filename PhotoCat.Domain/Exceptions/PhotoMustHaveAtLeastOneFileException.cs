namespace PhotoCat.Domain.Exceptions;

public sealed class PhotoMustHaveAtLeastOneFileException() 
    : DomainException("A photo must have at least one file.")
{        
}

