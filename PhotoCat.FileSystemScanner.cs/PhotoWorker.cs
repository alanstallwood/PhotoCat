using PhotoCat.FileSystemScanner.Services;

namespace PhotoCat.FileSystemScanner;

public class PhotoWorker(PhotoSyncWorker photoSyncWorker, FileSystemMonitor fileSystemMonitor, ILogger<PhotoWorker> logger) : BackgroundService
{
    private readonly PhotoSyncWorker _photoSyncWorker = photoSyncWorker;
    private readonly FileSystemMonitor _fileSystemMonitor = fileSystemMonitor;
    private readonly ILogger<PhotoWorker> _logger = logger;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Start real-time monitoring
        _fileSystemMonitor.Start();

        _logger.LogInformation("PhotoWorker started.");

        // Periodic full scan as backup
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting backup full scan...");
                await _photoSyncWorker.RunFullScanAsync();
                _logger.LogInformation("Full scan completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during full scan");
            }

            // TODO: Delay between full scans (configurable)
            await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _fileSystemMonitor.Stop();
        _logger.LogInformation("PhotoWorker stopping.");
        return base.StopAsync(cancellationToken);
    }
}
