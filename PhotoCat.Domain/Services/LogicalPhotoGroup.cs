using PhotoCat.Domain.Photos;

namespace PhotoCat.Domain.Services;

public sealed class LogicalPhotoGroup
{
    public string GroupKey { get; private set; }
    private readonly List<NewFileDto> _files = [];

    public IReadOnlyCollection<NewFileDto> Files => _files;

    public LogicalPhotoGroup(NewFileDto first, string keyCandidate)
    {
        GroupKey = keyCandidate;
        _files.Add(first);
    }

    public bool TryAdd(NewFileDto file, string keyCandidate)
    {
        if (!GroupKeyService.BelongToSameLogicalPhoto(GroupKey, keyCandidate))
            return false;

        GroupKey = GroupKeyService.GetBestGroupKey(GroupKey, keyCandidate);
        _files.Add(file);
        return true;
    }

    public void Merge(LogicalPhotoGroup other)
    {
        foreach (var file in other._files)
        {
            TryAdd(file, GroupKeyService.NormalizeAndRemoveModifiers(file.FileName));
        }
    }
}
