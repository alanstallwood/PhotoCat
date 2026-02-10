namespace PhotoCat.Domain.Photos
{
    public sealed class Photo
    {
        public Guid Id { get; private set; }
        public string FileName { get; private set; } = null!;
        public DateTime TakenAt { get; private set; }

        // Tags are a value object collection
        private readonly List<Tag> _tags = new();
        public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

        // EF needs a private constructor
        private Photo() { }

        public Photo(Guid id, string fileName, DateTime takenAt)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("FileName is required", nameof(fileName));

            Id = id;
            FileName = fileName;
            TakenAt = takenAt;
        }

        // Behaviour: add a tag
        public void AddTag(string tagText)
        {
            var tag = new Tag(tagText);

            if (_tags.Any(t => t.Value.Equals(tag.Value, StringComparison.OrdinalIgnoreCase)))
                return; // ignore duplicates within this photo

            _tags.Add(tag);
        }
    }
}
