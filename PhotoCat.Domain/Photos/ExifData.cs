using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoCat.Domain.Photos
{
    public sealed record ExifData
    {
        private readonly Dictionary<string, string> _data;

        public static ExifData Empty { get; } = new ExifData([]);

        public ExifData(IDictionary<string, string> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            // Defensive copy to guarantee immutability
            _data = new Dictionary<string, string>(source, StringComparer.OrdinalIgnoreCase);
        }

        private ExifData(Dictionary<string, string> data)
        {
            _data = data;
        }

        public bool Contains(string key)
            => _data.ContainsKey(key);
        public string? Get(string key) =>
            _data.TryGetValue(key, out var value) ? value : null;

        public T? Get<T>(string key, Func<string, T> parser)
        {
            if (!_data.TryGetValue(key, out var value))
                return default;

            try
            {
                return parser(value);
            }
            catch
            {
                return default;
            }
        }

        public IReadOnlyDictionary<string, string> All()
            => _data;


    }
}
