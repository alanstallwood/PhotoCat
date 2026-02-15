namespace PhotoCat.Application
{
    public interface IChecksumService
    {
        Task<byte[]> CalculateAsync(string filePath, CancellationToken cancellationToken = default);
    }
}
