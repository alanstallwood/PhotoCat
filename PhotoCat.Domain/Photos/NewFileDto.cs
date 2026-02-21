namespace PhotoCat.Domain.Photos;

public record NewFileDto(
    string FileName,
    string FilePath,
    PhotoFileType FileType,
    byte[] Checksum,
    long SizeBytes,
    PhotoMetadata? Metadata = null
);