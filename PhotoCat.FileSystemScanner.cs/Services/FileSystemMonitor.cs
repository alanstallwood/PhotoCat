namespace PhotoCat.FileSystemScanner.Services;

public class FileSystemMonitor(PhotoSyncWorker photoSyncWorker, string basePath = "/photos")
{
    private readonly string _basePath = basePath;
    private readonly PhotoSyncWorker _photoSyncWorker = photoSyncWorker;
    private FileSystemWatcher? _watcher;

    public void Start()
    {
        _watcher = new FileSystemWatcher(_basePath)
        {
            IncludeSubdirectories = true,
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime
        };

        _watcher.Created += OnCreated;
        _watcher.Changed += OnChanged;
        _watcher.Deleted += OnDeleted;
    }

    public void Stop()
    {
        if (_watcher != null)
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
        }
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        _photoSyncWorker.HandleSingleFile(e.FullPath).Wait();
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        _photoSyncWorker.HandleSingleFile(e.FullPath).Wait();
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
        _photoSyncWorker.HandleDeletedFile(e.FullPath).Wait();
    }
}
