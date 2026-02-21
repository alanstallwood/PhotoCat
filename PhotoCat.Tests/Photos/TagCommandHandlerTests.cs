using PhotoCat.Application.Exceptions;
using PhotoCat.Application.Photos;
using PhotoCat.Application.Photos.AddTag;
using PhotoCat.Application.Photos.RemoveTag;
using PhotoCat.Domain.Photos;

namespace PhotoCat.Tests.Photos;

public sealed class TagCommandHandlerTests
{
    [Fact]
    public async Task AddTagHandle_ShouldAppendNormalizedTagAndPersist()
    {
        var photo = Photo.Create(tags: ["travel"]);
        var repository = new FakePhotoRepository(photo);
        var sut = new AddTagCommandHandler(repository);

        await sut.Handle(new AddTagCommand { PhotoId = photo.Id, Tag = " Nature " }, CancellationToken.None);

        Assert.Contains(photo.Tags, t => t.Name == "nature");
        Assert.Equal(1, repository.UpdateCallCount);
    }

    [Fact]
    public async Task RemoveTagHandle_ShouldDeleteTagAndPersist()
    {
        var photo = Photo.Create(tags: ["travel", "nature"]);
        var repository = new FakePhotoRepository(photo);
        var sut = new RemoveTagCommandHandler(repository);

        await sut.Handle(new RemoveTagCommand { PhotoId = photo.Id, Tag = " Nature " }, CancellationToken.None);

        Assert.DoesNotContain(photo.Tags, t => t.Name == "nature");
        Assert.Equal(1, repository.UpdateCallCount);
    }

    [Fact]
    public async Task AddTagHandle_ShouldThrowWhenPhotoDoesNotExist()
    {
        var repository = new FakePhotoRepository(photo: null);
        var sut = new AddTagCommandHandler(repository);

        await Assert.ThrowsAsync<PhotoNotFoundException>(() =>
            sut.Handle(new AddTagCommand { PhotoId = Guid.NewGuid(), Tag = "nature" }, CancellationToken.None));

        Assert.Equal(0, repository.UpdateCallCount);
    }

    private sealed class FakePhotoRepository(Photo? photo) : IPhotoRepository
    {
        private readonly Photo? _photo = photo;

        public int UpdateCallCount { get; private set; }

        public Task<Photo?> GetByIdAsync(Guid id, CancellationToken ct) => Task.FromResult(_photo);

        public Task UpdateAsync(Photo photo, CancellationToken ct)
        {
            UpdateCallCount++;
            return Task.CompletedTask;
        }

        public Task<bool> FileChecksumExistsAsync(byte[] checksum, CancellationToken ct) => Task.FromResult(false);
        public Task<Guid> AddAsync(Photo photo, CancellationToken ct) => Task.FromResult(photo.Id);
        public Task<Photo?> GetByChecksumAsync(byte[] checksum, CancellationToken ct) => Task.FromResult<Photo?>(null);
        public Task<Photo?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken ct) => Task.FromResult(_photo);
    }
}
