using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoCat.Domain.Photos
{
    public sealed class GeoLocation
    {
        decimal Latitude { get; set; }
        decimal Longitude { get; set; }
        decimal Altitude { get; set; }
    }
}
