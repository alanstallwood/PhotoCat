using PhotoCat.Application;
using PhotoCat.Application.Exceptions;
using PhotoCat.Application.Interfaces;
using PhotoCat.Application.Photos;
using PhotoCat.Application.Photos.AddPhoto;
using PhotoCat.Domain.Photos;
using PhotoCat.Infrastructure.Metadata;
using PhotoCat.Infrastructure.Photos;

namespace PhotoCat.Tests.Photos;

public sealed class AddPhotoHandlerTests
{
    [Fact]
    public async Task Handle_ShouldPersistPhotoAndSaveChanges()
    {
        using var tempFile = new TempFile();
        var repository = new FakePhotoRepository();
        var metadata = new PhotoMetadata {
            DateTaken = new DateTime(2024, 1, 1),
            Camera = null,
            Exposure = null,
            Dimensions = null,
            Location = null,
            RawExif = null 
        };
        var exifExtractor = new FakeExifExtractor(metadata);
        var checksumService = new FakeChecksumService();
        var fileTypeDetector = new FakeFileTypeDetector();

        var handler = new AddPhotoCommandHandler(exifExtractor, checksumService, fileTypeDetector, repository);
        var command = new AddPhotoCommand(
            fullFilePaths: [tempFile.Path],
            tags: ["Travel", "travel"] );

        var id = await handler.Handle(command);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repository.LastAddedPhoto);
        Assert.Equal(id, repository.LastAddedPhoto!.Id);
        Assert.Single(repository.LastAddedPhoto.Tags);
        Assert.Equal(1, repository.AddCallCount);
        Assert.Equal(1, exifExtractor.CallCount);
    }

    [Fact]
    public async Task Handle_ShouldThrowWhenFileDoesNotExist()
    {
        var repository = new FakePhotoRepository();
        var exifExtractor = new FakeExifExtractor(null);
        var checksumService = new FakeChecksumService();
        var fileTypeDetector = new FakeFileTypeDetector();

        var handler = new AddPhotoCommandHandler(exifExtractor, checksumService, fileTypeDetector, repository);
        var command = new AddPhotoCommand(fullFilePaths: ["/path/does/not/exist.jpg"]);

        await Assert.ThrowsAsync<FileNotFoundApplicationException>(() => handler.Handle(command));

        Assert.Equal(0, repository.AddCallCount);
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

        public PhotoMetadata? Extract(Stream fileStream, string fileName)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class FakePhotoRepository : IPhotoRepository
    {
        public int AddCallCount { get; private set; }
        public Photo? LastAddedPhoto { get; private set; }

        public Task<Photo?> GetByIdAsync(Guid id, CancellationToken ct)
            => Task.FromResult<Photo?>(null);

        public Task<Photo?> GetByChecksumAsync(byte[] checksum, CancellationToken ct)
        {
            return Task.FromResult<Photo?>(null);
        }

        Task<Guid> IPhotoRepository.AddAsync(Photo photo, CancellationToken ct)
        {
            AddCallCount++;
            LastAddedPhoto = photo;
            return Task.FromResult(photo.Id);
        }

        public Task UpdateAsync(Photo photo, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<bool> FileChecksumExistsAsync(byte[] checksum, CancellationToken ct)
        {
            return Task.FromResult(false);
        }

        public Task<Photo?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class FakeChecksumService : IChecksumService
    {
        public Task<byte[]> CalculateAsync(string filePath, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new byte[] { 0x00, 0x01, 0x02, 0x03 });
        }

        public Task<byte[]> CalculateAsync(Stream fileStream, CancellationToken ct = default)
        {
            return Task.FromResult(new byte[] { 0x00, 0x01, 0x02, 0x03 });
        }
    }

    private sealed class FakeFileTypeDetector : IFileTypeDetector
    {
        public PhotoFileType Detect(string filePath)
        {
            return PhotoFileType.Jpeg;
        }

        public PhotoFileType Detect(Stream stream)
        {
            return PhotoFileType.Jpeg;
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
