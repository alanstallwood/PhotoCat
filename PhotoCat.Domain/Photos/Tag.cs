using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoCat.Domain.Photos
{
    public sealed record Tag
    {
        public string Name { get; }

        public Tag(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tag cannot be empty", nameof(name));

            Name = name.Trim().ToLowerInvariant(); // normalisation
        }
    }
}
