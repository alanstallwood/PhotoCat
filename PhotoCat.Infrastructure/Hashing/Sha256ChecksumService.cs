using PhotoCat.Application;
using System.Security.Cryptography;

namespace PhotoCat.Infrastructure.Hashing;

public class Sha256ChecksumService : IChecksumService
{
    public async Task<byte[]> CalculateAsync(string filePath, CancellationToken ct = default)
    {
        await using var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920,
            useAsync: true);

        return await CalculateAsync(stream, ct);
    }

    public async Task<byte[]> CalculateAsync(Stream fileStream, CancellationToken ct = default)
    {
        using var sha = SHA256.Create();
        return await sha.ComputeHashAsync(fileStream, ct);
    }
}
