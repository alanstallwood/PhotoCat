namespace PhotoCat.Domain.Photos
{
    public sealed class GeoLocation(double? latitude, double? longitude, double? altitude)
    {
        public double? Latitude { get; set; } = latitude;
        public double? Longitude { get; set; } = longitude;
        public double? Altitude { get; set; } = altitude;
    }
}
