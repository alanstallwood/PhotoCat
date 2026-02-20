using PhotoCat.Domain.Photos;

namespace PhotoCat.Tests.Photos;

public sealed class PhotoTests
{
    [Fact]
    public void Create_ShouldNormalizeAndDeduplicateTags()
    {
        var photo = Photo.Create(
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
            tags: ["nature", "travel"]);

        photo.RemoveTag(" Nature ");

        Assert.DoesNotContain(photo.Tags, t => t.Name == "nature");
        Assert.Single(photo.Tags);
    }


}
