using PhotoCat.Domain.Photos;

namespace PhotoCat.Tests.Photos;

public sealed class PhotoTests
{
    [Fact]
    public void Create_ShouldNormalizeAndDeduplicateTags()
    {
        var photo = Photo.Create(
            fileName: "image.jpg",
            filePath: "/tmp/image.jpg",
            fileFormat: PhotoFileType.Jpeg,
            sizeBytes: 256,
            checksum: new byte[] { 0x00, 0x01, 0x02, 0x03 },
            tags: [" Nature ", "nature", "Travel"]);

        var tagNames = photo.Tags.Select(t => t.Name).ToArray();

        Assert.Equal(2, tagNames.Length);
        Assert.Contains("nature", tagNames);
        Assert.Contains("travel", tagNames);
    }

    [Fact]
    public void RemoveTag_ShouldUseNormalizedComparison()
    {
        var photo = Photo.Create(
            fileName: "image.jpg",
            filePath: "/tmp/image.jpg",
            fileFormat: PhotoFileType.Jpeg,
            sizeBytes: 256,
            checksum: new byte[] { 0x00, 0x01, 0x02, 0x03 },
            tags: ["nature", "travel"]);

        photo.RemoveTag(" Nature ");

        Assert.DoesNotContain(photo.Tags, t => t.Name == "nature");
        Assert.Single(photo.Tags);
    }

    [Fact]
    public void Create_ShouldThrow_WhenSizeBytesIsNegative()
    {
        var act = () => Photo.Create(
            fileName: "image.jpg",
            filePath: "/tmp/image.jpg",
            fileFormat: PhotoFileType.Jpeg,
            sizeBytes: -1,
            checksum: new byte[] { 0x00, 0x01, 0x02, 0x03 });

        Assert.Throws<ArgumentOutOfRangeException>(act);
    }
}
