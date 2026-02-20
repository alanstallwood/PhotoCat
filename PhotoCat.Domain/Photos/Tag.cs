using PhotoCat.Domain.Exceptions;

namespace PhotoCat.Domain.Photos;

public sealed record Tag
{
    public string Name { get; }

    public Tag(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new TagCannotBeEmptyException();

        Name = name.Trim().ToLowerInvariant(); // normalisation
    }
}
