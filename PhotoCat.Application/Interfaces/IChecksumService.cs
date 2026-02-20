namespace PhotoCat.Application
{
    public interface IChecksumService
    {
        Task<byte[]> CalculateAsync(string filePath, CancellationToken cancellationToken = default);
        Task<byte[]> CalculateAsync(Stream fileStream, CancellationToken ct = default);
    }
}
