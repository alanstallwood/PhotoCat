using PhotoCat.Domain.Photos;

namespace PhotoCat.Application.Photos
{
    internal class NewFileData(string name, string path, PhotoFileType fileType, byte[] checksum, long length, PhotoMetadata? metadata)
    {
        public string FileName { get; } = name;
        public string FilePath { get; } = path;
        public PhotoFileType FileType { get; } = fileType;
        public byte[] Checksum { get; } = checksum;
        public long SizeBytes { get; } = length;
        public PhotoMetadata? Metadata { get; } = metadata;
    }
}