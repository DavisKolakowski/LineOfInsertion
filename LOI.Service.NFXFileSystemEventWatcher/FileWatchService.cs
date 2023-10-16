namespace LOI.Service.NFXFileSystemEventWatcher
{
    using Serilog;

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FileWatchService
    {
        private readonly ILogger<FileWatchService> _logger;

        public FileWatchService(ILogger<FileWatchService> logger)
        {
            this._logger = logger;
        }

        public FileSystemWatcher BuildWatcher(string path, string filter = "*.*")
        {
            var formatLocalPath = GetDirectoryInfo(path);

            if (!formatLocalPath.Exists)
            {
                this._logger.LogWarning($"Directory {path} does not exist.");
            }

            return new FileSystemWatcher
            {
                Path = formatLocalPath.FullName,
                Filter = filter,
                NotifyFilter = NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName,
            };
        }

        public FileSystemWatcher EnableEvents(FileSystemWatcher watcher)
        {
            watcher.Created += OnChanged;
            watcher.Changed += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            watcher.EnableRaisingEvents = true;
            return watcher;
        }

        public FileSystemWatcher DisableEvents(FileSystemWatcher watcher)
        {
            watcher.Created -= OnChanged;
            watcher.Changed -= OnChanged;
            watcher.Deleted -= OnChanged;
            watcher.Renamed -= OnRenamed;
            watcher.Error -= OnError;

            watcher.EnableRaisingEvents = false;
            return watcher;
        }

        public void DeleteWatcher(FileSystemWatcher watcher)
        {
            watcher.Dispose();
        }

        private DirectoryInfo GetDirectoryInfo(string path)
        {
            return new DirectoryInfo(path);
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            this._logger.LogInformation($"File: {e.FullPath} {e.ChangeType}");
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            this._logger.LogInformation($"File: {e.OldFullPath} renamed to {e.FullPath}");
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            this._logger.LogError($"Error: {e.GetException()}");
        }
    }
}
