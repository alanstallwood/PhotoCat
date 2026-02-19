using MediatR;
using PhotoCat.Application.Exceptions;
using PhotoCat.Application.Interfaces;
using PhotoCat.Application.Photos.AddPhoto;
using PhotoCat.Domain.Photos;
using PhotoCat.Infrastructure.Metadata;
using PhotoCat.Infrastructure.Photos;

namespace PhotoCat.Application.Photos;

public sealed class AddPhotoCommandHandler(IExifExtractor exifExtractor, IChecksumService checksumService, 
                                        IFileTypeDetector fileTypeDetector, IPhotoRepository photoRepository) 
    : IRequestHandler<AddPhotoCommand, AddPhotoResult>
{
    private readonly IExifExtractor _exifExtractor = exifExtractor;
    private readonly IChecksumService _checksumService = checksumService;
    private readonly IFileTypeDetector _fileTypeDetector = fileTypeDetector;
    private readonly IPhotoRepository _photoRepository = photoRepository;

    public async Task<AddPhotoResult> Handle(AddPhotoCommand request, CancellationToken ct = default)
    {
        if (request.FilePaths == null || request.FilePaths.Count == 0)
            throw new NoFilesProvidedException();

        var newFiles = new List<(string FileName, string FilePath, PhotoFileType FileType, byte[] Checksum, long? SizeBytes, PhotoMetadata? metadata)>();
        foreach (var filePath in request.FilePaths)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundApplicationException(filePath);

            var checksum = await _checksumService.CalculateAsync(filePath, ct);
            if (await _photoRepository.FileChecksumExistsAsync(checksum, ct))
            {
                continue;
            }

            var fileInfo = new FileInfo(filePath);
            var fileType = _fileTypeDetector.Detect(filePath);
            var metadata = _exifExtractor.Extract(filePath);

            newFiles.Add((fileInfo.Name, filePath, fileType, checksum, fileInfo.Length, metadata));
        }

        if (newFiles.Count == 0)
            throw new AllFilesAlreadyExistException();

        var mainFile = newFiles
            .FirstOrDefault(f => f.FileType == PhotoFileType.Nef);

        if(mainFile == default)
        {
            mainFile = newFiles.First();
        }

        // Create the aggregate using the factory
        var photo = Photo.Create(
            mainFile.metadata,
            request.Tags);

        newFiles.ForEach(f =>
        {
            var file = photo.AddFile(
                f.FileName,
                f.FilePath,
                f.FileType,
                f.SizeBytes,
                f.Checksum,
                f.metadata);

            if (f == mainFile)
            {
                photo.SetRepresentativeFile(file.Id);
            }
        });

        await _photoRepository.AddAsync(photo, ct);

        return AddPhotoResult.Created(photo.Id);
    }
}
