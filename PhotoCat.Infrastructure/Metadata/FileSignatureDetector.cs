using PhotoCat.Application.Interfaces;
using PhotoCat.Domain.Photos;

namespace PhotoCat.Infrastructure.Metadata;

public sealed class FileSignatureDetector : IFileTypeDetector
{
    public PhotoFileType Detect(Stream stream)
    {
        Span<byte> header = stackalloc byte[12];
        stream.ReadExactly(header);

        // JPEG: FF D8 FF
        if (header[0] == 0xFF &&
            header[1] == 0xD8 &&
            header[2] == 0xFF)
            return PhotoFileType.Jpeg;

        // PNG: 89 50 4E 47
        if (header[0] == 0x89 &&
            header[1] == 0x50 &&
            header[2] == 0x4E &&
            header[3] == 0x47)
            return PhotoFileType.Png;

        // GIF: 47 49 46 38
        if (header[0] == 0x47 &&
            header[1] == 0x49 &&
            header[2] == 0x46 &&
            header[3] == 0x38)
            return PhotoFileType.Gif;

        // WebP: "RIFF....WEBP"
        if (header[0] == 0x52 &&
            header[1] == 0x49 &&
            header[2] == 0x46 &&
            header[3] == 0x46 &&
            header[8] == 0x57 &&
            header[9] == 0x45 &&
            header[10] == 0x42 &&
            header[11] == 0x50)
            return PhotoFileType.WebP;

        // NEF (Nikon RAW) = TIFF-based
        // TIFF header: 49 49 2A 00 OR 4D 4D 00 2A
        if ((header[0] == 0x49 && header[1] == 0x49 &&
             header[2] == 0x2A && header[3] == 0x00) ||
            (header[0] == 0x4D && header[1] == 0x4D &&
             header[2] == 0x00 && header[3] == 0x2A))
            return PhotoFileType.Nef;

        return PhotoFileType.Unknown;
    }

    public PhotoFileType Detect(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        return Detect(stream);
    }
}
