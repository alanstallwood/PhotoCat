using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoCat.Domain.Photos
{
    public sealed class CameraInfo
    {
        string Make { get; set; }
        string Model { get; set; }
        string? Lens { get; set; }
    }
}
