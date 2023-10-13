namespace LOI.Service.NFXFileSystemEventWatcher
{
    public class NFXFileSystemWatcher : IDisposable
    {
        private readonly FileSystemWatcher _watcher;
        private readonly string _path;
        private readonly string _filter;

        public NFXFileSystemWatcher(string path, string filter)
        {
            this._path = path;
            this._filter = filter;

            this._watcher = new FileSystemWatcher
            {
                Path = this._path,
                Filter = this._filter,
                NotifyFilter = NotifyFilters.LastAccess 
                    | NotifyFilters.LastWrite
                    | NotifyFilters.FileName 
                    | NotifyFilters.DirectoryName,
            };

            this._watcher.Created += OnChanged;
            this._watcher.Changed += OnChanged;
            this._watcher.Deleted += OnChanged;
            this._watcher.Renamed += OnRenamed;
        }

        public void Start()
        {
            this._watcher.EnableRaisingEvents = true;
        }

        public void Pause()
        {
            this._watcher.EnableRaisingEvents = false;
        }

        public void Resume()
        {
            this._watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
        }

        public void Dispose()
        {
            this._watcher.Dispose();
        }
    }
}