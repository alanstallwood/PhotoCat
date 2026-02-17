using System.Globalization;
using PhotoCat.Domain.Photos;

namespace PhotoCat.Infrastructure.Metadata
{
    public sealed class MetadataMapper : IMetadataMapper
    {
        public static PhotoMetadata Map(IDictionary<string, string> rawExif)
        {
            var metadata = new PhotoMetadata
            {
                DateTaken = ParseDate(rawExif),
                Camera = ParseCameraInfo(rawExif),
                Exposure = ParseExposureInfo(rawExif),
                Dimensions = ParseDimensions(rawExif),
                Location = ParseGeoLocation(rawExif),
                RawExif = rawExif.AsReadOnly()
            };
            return metadata;
        }

        private static DateTime? ParseDate(IDictionary<string, string> exif)
        {
            var dateStr = GetValue(exif, "Date/Time Original") ??
                          GetValue(exif, "DateTimeOriginal");

            if (DateTime.TryParseExact(
                    dateStr,
                    ["yyyy:MM:dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss"],
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeLocal,
                    out var exifDate))
                return exifDate;

            if (DateTime.TryParse(dateStr, out var date))
                return date;

            return null;
        }

        private static CameraInfo? ParseCameraInfo(IDictionary<string, string> exif)
        {
            var make = GetValue(exif, "Make");
            var model = GetValue(exif, "Model");
            var lens = GetValue(exif, "Lens Model");

            if (make is null && model is null && lens is null)
                return null;

            return new CameraInfo(make, model, lens);
        }

        private static ExposureInfo? ParseExposureInfo(IDictionary<string, string> exif)
        {
            var iso = GetValue(exif, "ISOSpeedRatings") ??
                            GetValue(exif, "PhotographicSensitivity");

            var fNumberString = GetValue(exif, "F-Number");
            var focalLengthString = GetValue(exif, "Focal Length");
            var exposureTime = GetValue(exif, "Exposure Time");

            decimal? fNumber = decimal.TryParse(fNumberString, out var fNumberValue)
                ? fNumberValue
                : null;

            decimal? focalLength = decimal.TryParse(focalLengthString, out var focalLengthValue)
                ? focalLengthValue
                : null;

            if (iso is null &&
                fNumber is null &&
                focalLength is null &&
                exposureTime is null)
                return null;

            return new ExposureInfo(iso, fNumber, exposureTime, focalLength);
        }


        private static Dimensions? ParseDimensions(IDictionary<string, string> exif)
        {
            var widthStr = GetValue(exif, "Exif Image Width");
            var heightStr = GetValue(exif, "Exif Image Height");
            var orientationStr = GetValue(exif, "Orientation");

            int? width = int.TryParse(widthStr, out var w) ? w : null;
            int? height = int.TryParse(heightStr, out var h) ? h : null;
            int? orientation = int.TryParse(orientationStr, out var o) ? o : null;

            if (width is null && height is null && orientation is null)
                return null;

            return new Dimensions(width, height, orientation);
        }


        private static GeoLocation? ParseGeoLocation(IDictionary<string, string> exif)
        {
            var latStr = GetValue(exif, "GPS Latitude");
            var lonStr = GetValue(exif, "GPS Longitude");
            var altStr = GetValue(exif, "GPS Altitude");

            double? latitude = double.TryParse(latStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var lat) ? lat : null;
            double? longitude = double.TryParse(lonStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var lon) ? lon : null;
            double? altitude = double.TryParse(altStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var alt) ? alt : null;

            if (latitude is null && longitude is null && altitude is null)
                return null;

            return new GeoLocation(latitude, longitude, altitude);
        }


        private static string? GetValue(IDictionary<string, string> exif, string key)
        {
            if (exif.TryGetValue(key, out var value) &&
                !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            foreach (var kvp in exif)
            {
                if (!kvp.Key.EndsWith($":{key}", StringComparison.OrdinalIgnoreCase) ||
                    string.IsNullOrWhiteSpace(kvp.Value))
                {
                    continue;
                }

                return kvp.Value;
            }

            return null;
        }

    }
}
