using PhotoCat.Application;
using PhotoCat.Application.Photos;
using PhotoCat.Application.Photos.AddPhoto;
using PhotoCat.Domain.Photos;
using PhotoCat.Infrastructure.Metadata;

namespace PhotoCat.Tests.Photos;

public sealed class AddPhotoHandlerTests
{
    [Fact]
    public async Task Handle_ShouldPersistPhotoAndSaveChanges()
    {
        using var tempFile = new TempFile();
        var repository = new FakePhotoRepository();
        var unitOfWork = new FakeUnitOfWork();
        var metadata = new PhotoMetadata(
            DateTaken: new DateTime(2024, 1, 1),
            Camera: null,
            Exposure: null,
            Dimensions: null,
            Location: null,
            RawExif: null);
        var exifExtractor = new FakeExifExtractor(metadata);

        var handler = new AddPhotoHandler(exifExtractor, repository, unitOfWork);
        var command = new AddPhotoCommand(
            fileName: "sample.jpg",
            filePath: tempFile.Path,
            fileFormat: "jpg",
            sizeBytes: 42,
            checksum: "sha256",
            tags: ["Travel", "travel"]);

        var id = await handler.Handle(command);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repository.LastAddedPhoto);
        Assert.Equal(id, repository.LastAddedPhoto!.Id);
        Assert.Equal("sample.jpg", repository.LastAddedPhoto.FileName);
        Assert.Equal(1, repository.LastAddedPhoto.Tags.Count);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
        Assert.Equal(1, repository.AddCallCount);
        Assert.Equal(1, exifExtractor.CallCount);
    }

    [Fact]
    public async Task Handle_ShouldThrowWhenFileDoesNotExist()
    {
        var repository = new FakePhotoRepository();
        var unitOfWork = new FakeUnitOfWork();
        var exifExtractor = new FakeExifExtractor(null);

        var handler = new AddPhotoHandler(exifExtractor, repository, unitOfWork);
        var command = new AddPhotoCommand(
            fileName: "missing.jpg",
            filePath: "/path/does/not/exist.jpg",
            fileFormat: "jpg",
            sizeBytes: 42,
            checksum: "sha256");

        await Assert.ThrowsAsync<FileNotFoundException>(() => handler.Handle(command));

        Assert.Equal(0, repository.AddCallCount);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
        Assert.Equal(0, exifExtractor.CallCount);
    }

    private sealed class FakeExifExtractor(PhotoMetadata? metadata) : IExifExtractor
    {
        public int CallCount { get; private set; }

        public PhotoMetadata? Extract(string filePath)
        {
            CallCount++;
            return metadata;
        }
    }

    private sealed class FakePhotoRepository : IPhotoRepository
    {
        public int AddCallCount { get; private set; }
        public Photo? LastAddedPhoto { get; private set; }

        public Task<Photo?> GetByIdAsync(Guid id, CancellationToken ct)
            => Task.FromResult<Photo?>(null);

        public Task AddAsync(Photo photo, CancellationToken ct)
        {
            AddCallCount++;
            LastAddedPhoto = photo;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken ct)
        {
            SaveChangesCallCount++;
            return Task.FromResult(1);
        }
    }

    private sealed class TempFile : IDisposable
    {
        public string Path { get; }

        public TempFile()
        {
            Path = System.IO.Path.GetTempFileName();
        }

        public void Dispose()
        {
            if (File.Exists(Path))
            {
                File.Delete(Path);
            }
        }
    }
}
