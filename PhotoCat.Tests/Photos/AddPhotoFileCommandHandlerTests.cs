using PhotoCat.Application.Exceptions;
using PhotoCat.Application.Interfaces;
using PhotoCat.Application.Photos.AddPhotoFile;
using PhotoCat.Domain.Photos;

namespace PhotoCat.Tests.Photos;

public sealed class AddPhotoFileCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldSaveFileAndUpdatePhoto()
    {
        using var tempDir = new TempDirectory();
        var photo = CreatePhotoWithExistingFile(tempDir.Path);
        var repository = new FakePhotoRepository(photo);
        var exifExtractor = new FakeExifExtractor();
        var checksumService = new FakeChecksumService();
        var fileTypeDetector = new FakeFileTypeDetector();

        var content = new byte[] { 1, 2, 3, 4, 5 };
        await using var stream = new MemoryStream(content);
        var command = new AddPhotoFileCommand(photo.Id, stream, "new-file.jpg");
        var sut = new AddPhotoFileCommandHandler(exifExtractor, checksumService, fileTypeDetector, repository);

        var fileId = await sut.Handle(command, CancellationToken.None);
        var savedPath = Path.Combine(tempDir.Path, "new-file.jpg");

        Assert.NotEqual(Guid.Empty, fileId);
        Assert.True(File.Exists(savedPath));
        Assert.Equal(content, await File.ReadAllBytesAsync(savedPath));
        Assert.Equal(1, repository.UpdateCallCount);
        Assert.Equal(1, exifExtractor.CallCount);
        Assert.Equal(1, fileTypeDetector.CallCount);
        Assert.Equal(2, photo.Files.Count);
    }

    [Fact]
    public async Task Handle_ShouldThrowWhenPhotoDoesNotExist()
    {
        var repository = new FakePhotoRepository(photo: null);
        var sut = new AddPhotoFileCommandHandler(new FakeExifExtractor(), new FakeChecksumService(), new FakeFileTypeDetector(), repository);

        await using var stream = new MemoryStream([1, 2, 3]);
        var command = new AddPhotoFileCommand(Guid.NewGuid(), stream, "new-file.jpg");

        await Assert.ThrowsAsync<PhotoNotFoundException>(() => sut.Handle(command, CancellationToken.None));
        Assert.Equal(0, repository.UpdateCallCount);
    }

    private static Photo CreatePhotoWithExistingFile(string filePath)
    {
        var photo = Photo.Create();
        photo.AddFile("existing.jpg", filePath, PhotoFileType.Jpeg, 10, [0x10], null);
        return photo;
    }

    private sealed class FakePhotoRepository(Photo? photo) : IPhotoRepository
    {
        private readonly Photo? _photo = photo;

        public int UpdateCallCount { get; private set; }

        public Task<Photo?> GetByIdAsync(Guid id, CancellationToken ct) => Task.FromResult(_photo);
        public Task<Photo?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken ct) => Task.FromResult(_photo);
        public Task<Photo?> GetByChecksumAsync(byte[] checksum, CancellationToken ct) => Task.FromResult<Photo?>(null);
        public Task<bool> FileChecksumExistsAsync(byte[] checksum, CancellationToken ct) => Task.FromResult(false);
        public Task<Guid> AddAsync(Photo photo, CancellationToken ct) => Task.FromResult(photo.Id);

        public Task UpdateAsync(Photo photo, CancellationToken ct)
        {
            UpdateCallCount++;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeExifExtractor : IExifExtractor
    {
        public int CallCount { get; private set; }

        public PhotoMetadata? Extract(string filePath) => throw new NotSupportedException();

        public PhotoMetadata? Extract(Stream fileStream, string fileName)
        {
            CallCount++;
            return null;
        }
    }

    private sealed class FakeChecksumService : IChecksumService
    {
        public Task<byte[]> CalculateAsync(string filePath, CancellationToken cancellationToken = default)
            => Task.FromResult(new byte[] { 0x01, 0x02, 0x03 });

        public Task<byte[]> CalculateAsync(Stream fileStream, CancellationToken ct = default)
            => Task.FromResult(new byte[] { 0x01, 0x02, 0x03 });
    }

    private sealed class FakeFileTypeDetector : IFileTypeDetector
    {
        public int CallCount { get; private set; }

        public PhotoFileType Detect(string filePath) => throw new NotSupportedException();

        public PhotoFileType Detect(Stream stream)
        {
            CallCount++;
            return PhotoFileType.Jpeg;
        }
    }

    private sealed class TempDirectory : IDisposable
    {
        public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"photocat-tests-{Guid.NewGuid():N}");

        public TempDirectory()
        {
            Directory.CreateDirectory(Path);
        }

        public void Dispose()
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, recursive: true);
            }
        }
    }
}
