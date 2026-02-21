using PhotoCat.Domain.Exceptions;
using PhotoCat.Domain.Services;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PhotoCat.Infrastructure")]
namespace PhotoCat.Domain.Photos;

public sealed class Photo
{
    public Guid Id { get; private set; }
    public DateTime? DateTaken { get; private set; }

    public string GroupKey { get; private set; } = null!;

    public CameraInfo? Camera { get; private set; } = null!;
    public ExposureInfo? Exposure { get; private set; }
    public GeoLocation? Location { get; private set; }

    public IReadOnlyDictionary<string, string>? RawExif { get; init; }

    private readonly List<PhotoFile> _files = [];
    public IReadOnlyCollection<PhotoFile> Files => _files.Where(f => !f.IsDeleted).ToList().AsReadOnly();

    public Guid RepresentativeFileId { get; private set; }

    // Tags are a value object collection
    private readonly List<Tag> _tags = [];
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

    public bool IsDeleted { get; private set; }

    // DB-managed audit fields
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }


    // EF needs a private constructor
    private Photo() { }

    private Photo(PhotoMetadata? metadata = null)
    {
        Id = Guid.NewGuid();
        DateTaken = metadata?.DateTaken;
        GroupKey = string.Empty;
        Camera = metadata?.Camera;
        Exposure = metadata?.Exposure;
        Location = metadata?.Location;
        RawExif = metadata?.RawExif;
    }

    public static Photo Create(IEnumerable<NewFileDto> initialFiles, IEnumerable<string>? tags = null)
    {
        if (initialFiles == null || !initialFiles.Any())
            throw new PhotoMustHaveAtLeastOneFileException();

        var mainFile = initialFiles.FirstOrDefault(f => f.FileType == PhotoFileType.Nef);
        if (mainFile == default)
        {
            mainFile = initialFiles.First();
        }

        var photo = new Photo(mainFile.Metadata);

        foreach (var file in initialFiles)
        {
            try
            {
                var result = photo.AddFile(file);
                if(file == mainFile)
                {
                    photo.SetRepresentativeFile(result.Id);
                }
            }
            catch (PhotoFilesMustBeUniqueException)
            {
                //Ignore duplicates files in this entity. We should end up with one file if all the same.
            }
        }

        if (tags != null)
        {
            foreach (var tagName in tags)
                photo.AddTag(tagName);
        }

        return photo;
    }

    /// <summary>
    /// Used for rehydration by repo
    /// </summary>
    internal Photo(
        Guid id,
        DateTime? dateTaken,
        string groupKey,
        CameraInfo? camera,
        ExposureInfo? exposure,
        GeoLocation? location,
        IReadOnlyDictionary<string, string>? rawExif,
        IEnumerable<PhotoFile>? files,
        Guid representativeFileId,
        IEnumerable<Tag>? tags,
        bool isDeleted,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        DateTaken = dateTaken;
        GroupKey = groupKey;
        Camera = camera;
        Exposure = exposure;
        Location = location;
        RawExif = rawExif;

        if (files != null)
            _files.AddRange(files);

        RepresentativeFileId = representativeFileId;

        if (tags != null)
            _tags.AddRange(tags);

        IsDeleted = isDeleted;

        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public PhotoFile AddFile(NewFileDto newFile)
    {
        if(_files.Any(f => f.Checksum.SequenceEqual(newFile.Checksum)))
        {
            throw new PhotoFilesMustBeUniqueException();
        }

        var photoFile = PhotoFile.Create(newFile.FileName, newFile.FilePath, newFile.FileType, 
                                                newFile.SizeBytes, newFile.Checksum, newFile.Metadata);
        _files.Add(photoFile);

        GroupKey = GroupKeyService.GetBestGroupKey(GroupKey, newFile.FileName);

        return photoFile;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }

    public void Restore()
    {
        IsDeleted = false;
        if (Files.Count == 0)
        {
            _files.OrderByDescending(f => f.UpdatedAt).First().Restore();
        }
    }

    public void AddTag(string tagName)
    {
        var tag = new Tag(tagName);

        if (_tags.Any(t => t.Name == tag.Name))
            return; // ignore duplicates within this photo

        _tags.Add(tag);
    }

    public void RemoveTag(string tagName)
    {
        var normalized = tagName.Trim().ToLowerInvariant();
        var existing = _tags.FirstOrDefault(t => t.Name == normalized);
        if (existing != null)
        {
            _tags.Remove(existing);
        }
    }

    public void SetRepresentativeFile(Guid newRepresentativeFileId)
    {
        var file = _files.SingleOrDefault(f => f.Id == newRepresentativeFileId);
        if(file is null)
        {
            return;
        }
        RepresentativeFileId = newRepresentativeFileId;
    }

    public void SoftDeleteFile(Guid photoFileId)
    {
        var file = _files.SingleOrDefault(f => f.Id == photoFileId);
        file?.SoftDelete();
        if (Files.Count == 0)
        {
            SoftDelete();
        }
    }

    public void RestoreFile(Guid photoFileId)
    {
        var file = _files.SingleOrDefault(f => f.Id == photoFileId);
        file?.Restore();
        IsDeleted = false; //Shoudn't be true but just in case
    }
}
