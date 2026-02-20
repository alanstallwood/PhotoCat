using PhotoCat.Infrastructure.Metadata;

namespace PhotoCat.Tests.Metadata;

public sealed class MetadataMapperTests
{
    [Fact]
    public void Map_ShouldParseValuesFromPrefixedExifKeys()
    {
        var exif = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Exif SubIFD:Date/Time Original"] = "2024:02:03 10:20:30",
            ["Exif IFD0:Make"] = "Canon",
            ["Exif IFD0:Model"] = "EOS R6",
            ["Exif SubIFD:F-Number"] = "2.8",
            ["Exif SubIFD:Focal Length"] = "35",
            ["Exif SubIFD:Exposure Time"] = "1/125",
            ["Exif SubIFD:Exif Image Width"] = "4000",
            ["Exif SubIFD:Exif Image Height"] = "3000",
            ["GPS:GPS Latitude"] = "43.6532",
            ["GPS:GPS Longitude"] = "-79.3832"
        };

        var metadata = MetadataMapper.Map(exif);

        Assert.Equal(new DateTime(2024, 2, 3, 10, 20, 30), metadata.DateTaken);
        Assert.NotNull(metadata.Camera);
        Assert.NotNull(metadata.Exposure);
        Assert.NotNull(metadata.Dimensions);
        Assert.NotNull(metadata.Location);
        Assert.NotNull(metadata.RawExif);
        Assert.Equal(10, metadata.RawExif!.Count);
    }

    [Fact]
    public void Map_ShouldLeaveSectionsNull_WhenTagsMissing()
    {
        var metadata = MetadataMapper.Map(new Dictionary<string, string>());

        Assert.Null(metadata.DateTaken);
        Assert.Null(metadata.Camera);
        Assert.Null(metadata.Exposure);
        Assert.Null(metadata.Dimensions);
        Assert.Null(metadata.Location);
        Assert.NotNull(metadata.RawExif);
        Assert.Empty(metadata.RawExif!);
    }
}
