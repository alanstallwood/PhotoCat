using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoCat.Domain.Photos
{
    public sealed class Tag
    {
        public string Value { get; }

        public Tag(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Tag cannot be empty", nameof(value));

            Value = value.Trim().ToLowerInvariant(); // normalisation
        }

        public override bool Equals(object? obj)
            => obj is Tag other && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();
    }
}
