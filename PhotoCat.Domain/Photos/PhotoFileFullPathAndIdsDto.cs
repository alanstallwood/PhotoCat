namespace PhotoCat.Domain.Photos;

public record PhotoFileFullPathAndIdsDto(
    Guid PhotoId,
    Guid FileId,
    string FullFilePath
);


