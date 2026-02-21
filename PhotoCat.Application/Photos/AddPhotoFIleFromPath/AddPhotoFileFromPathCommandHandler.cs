using MediatR;
using PhotoCat.Application.Exceptions;
using PhotoCat.Application.Interfaces;
using PhotoCat.Application.Photos.AddPhotoFileFromPath;
using PhotoCat.Domain.Photos;
using PhotoCat.Infrastructure.Metadata;

namespace PhotoCat.Application.Photos.AddPhotoFIleFromPath
{
    public sealed class AddPhotoFileFromPathCommandHandler(IExifExtractor exifExtractor, IChecksumService checksumService,
                                        IFileTypeDetector fileTypeDetector, IPhotoRepository photoRepository)
        : IRequestHandler<AddPhotoFileFromPathCommand, Guid>
    {
        private readonly IExifExtractor _exifExtractor = exifExtractor;
        private readonly IChecksumService _checksumService = checksumService;
        private readonly IFileTypeDetector _fileTypeDetector = fileTypeDetector;
        private readonly IPhotoRepository _photoRepository = photoRepository;

        public async Task<Guid> Handle(AddPhotoFileFromPathCommand request, CancellationToken ct)
        {
            var photo = await _photoRepository.GetByIdAsync(request.PhotoId, ct);
            if (photo == null)
            {
                throw new PhotoNotFoundException(request.PhotoId);
            }

            var fullPath = Path.GetFullPath(request.FilePath, request.FileName);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundApplicationException(fullPath);
            }

            var checksum = await _checksumService.CalculateAsync(fullPath, ct);
            if (await _photoRepository.FileChecksumExistsAsync(checksum, ct))
            {
                throw new FileAlreadyExistsException();
            }
            
            var metadata = _exifExtractor.Extract(fullPath);
            var fileType = _fileTypeDetector.Detect(fullPath);
            var fileInfo = new FileInfo(fullPath);

            var fileDto = new NewFileDto(
                request.FileName,
                request.FilePath,
                fileType,
                checksum,
                fileInfo.Length,
                metadata);

            var file = photo.AddFile(fileDto);
            await _photoRepository.UpdateAsync(photo, ct);

            return file.Id;
        }
    }
}
