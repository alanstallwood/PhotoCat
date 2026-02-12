using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoCat.Domain.Photos
{
    public sealed class ExposureInfo
    {
        int? Iso { get; set; }
        decimal? Apertuere { get; set; }
        string? ShutterSpeed { get; set; }
        decimal? FocalLength { get; set; }
    }
}
