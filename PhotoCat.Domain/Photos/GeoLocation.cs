namespace PhotoCat.Domain.Photos
{
    public sealed class GeoLocation(double? latitude, double? longitude, double? altitude)
    {
        double? Latitude { get; set; } = latitude;
        double? Longitude { get; set; } = longitude;
        double? Altitude { get; set; } = altitude;
    }
}
