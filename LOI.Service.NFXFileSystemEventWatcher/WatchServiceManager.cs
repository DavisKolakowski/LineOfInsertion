namespace LOI.Service.NFXFileSystemEventWatcher
{
    using Microsoft.AspNetCore.SignalR.Client;
    using Serilog;

    using System.Collections.Concurrent;

    public class WatchServiceManager
    {
        private readonly ILogger<WatchServiceManager> _logger;
        private readonly FileWatchService _fileSystemWatcherService;

        public WatchServiceManager(ILogger<WatchServiceManager> logger, FileWatchService fileSystemWatcherService)
        {
            this._logger = logger;
            this._fileSystemWatcherService = fileSystemWatcherService;
        }

        private readonly ConcurrentDictionary<string, FileSystemWatcher> _watchers = new ConcurrentDictionary<string, FileSystemWatcher>();

        public void AddWatcher(string path, string? filter)
        {
            var watcher = new FileSystemWatcher();

            if (string.IsNullOrEmpty(filter))
            {
                watcher = this._fileSystemWatcherService.BuildWatcher(path);
            }
            else
            {
                watcher = this._fileSystemWatcherService.BuildWatcher(path, filter);
            }

            if (this._watchers.TryAdd(watcher.Path, watcher))
            {
                this._fileSystemWatcherService.EnableEvents(watcher);
                this._logger.LogInformation($"Started watching path: {path} with filter: {filter}");
            }
        }

        public void DisableWatcher(string path)
        {
            if (this._watchers.TryGetValue(path, out var watcher))
            {
                var newWatcher = this._fileSystemWatcherService.DisableEvents(watcher);
                UpdateWatcher(path, newWatcher, watcher);
                this._logger.LogInformation($"Paused watching path: {path}");
            }
            this._logger.LogWarning($"Failed to pause watching path: {path}");
        }

        public void EnableWatcher(string path)
        {
            if (this._watchers.TryGetValue(path, out var watcher))
            {
                var newWatcher = this._fileSystemWatcherService.EnableEvents(watcher);
                UpdateWatcher(path, newWatcher, watcher);
                this._logger.LogInformation($"Resumed watching path: {path}");
            }
            this._logger.LogWarning($"Failed to resume watching path: {path}");
        }

        public void RemoveWatcher(string path)
        {
            if (this._watchers.TryRemove(path, out var watcher))
            {
                this._fileSystemWatcherService.DeleteWatcher(watcher);
                this._logger.LogInformation($"Stopped watching path: {path}");
            }
            this._logger.LogWarning($"Failed to stop watching path: {path}");
        }

        private void UpdateWatcher(string path, FileSystemWatcher newWatcher, FileSystemWatcher oldWatcher)
        {
            if (this._watchers.TryUpdate(path, newWatcher, oldWatcher))
            {
                this._logger.LogInformation("Updated watcher: {path}", path);
            }
            this._logger.LogWarning("Failed to update watcher: {path}", path);
        }
    }
}