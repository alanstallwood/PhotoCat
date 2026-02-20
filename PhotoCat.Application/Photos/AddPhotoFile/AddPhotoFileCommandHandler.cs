using MediatR;
using PhotoCat.Application.Exceptions;
using PhotoCat.Application.Interfaces;
using PhotoCat.Infrastructure.Metadata;

namespace PhotoCat.Application.Photos.AddPhotoFile;

public sealed class AddPhotoFileCommandHandler(IExifExtractor exifExtractor, IChecksumService checksumService,
                                    IFileTypeDetector fileTypeDetector, IPhotoRepository photoRepository) 
    : IRequestHandler<AddPhotoFileCommand, Guid>
{
    private readonly IExifExtractor _exifExtractor = exifExtractor;
    private readonly IChecksumService _checksumService = checksumService;
    private readonly IFileTypeDetector _fileTypeDetector = fileTypeDetector;
    private readonly IPhotoRepository _photoRepository = photoRepository;


    public async Task<Guid> Handle(AddPhotoFileCommand request, CancellationToken ct)
    {
        var photo = await _photoRepository.GetByIdAsync(request.PhotoId, ct);
        if (photo == null)
        {
            throw new PhotoNotFoundException(request.PhotoId);
        }

        var checksum = await _checksumService.CalculateAsync(request.File, ct);
        if (await _photoRepository.FileChecksumExistsAsync(checksum, ct))
        {
            throw new FileAlreadyExistsException();
        }

        var filePath = photo.Files.First().FilePath;
        string fullPath = Path.Combine(filePath, request.FileName);

        request.File.Seek(0, SeekOrigin.Begin);
        await using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
        {
            await request.File.CopyToAsync(fs, ct);
        }

        request.File.Seek(0, SeekOrigin.Begin);
        var metadata = _exifExtractor.Extract(request.File, request.FileName);
        request.File.Seek(0, SeekOrigin.Begin);
        var fileType = _fileTypeDetector.Detect(request.File);

        var file = photo.AddFile(request.FileName, filePath, fileType, request.File.Length, checksum, metadata);
        await _photoRepository.UpdateAsync(photo, ct);

        return file.Id;
    }
}
