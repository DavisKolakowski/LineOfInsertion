namespace LOI.Service.NFXFileSystemEventWatcher
{
    using Microsoft.AspNetCore.SignalR.Client;
    using Serilog;

    using System.Collections.Concurrent;

    public class ApplicationStateManager
    {
        private readonly ConcurrentDictionary<string, FileSystemWatcher> _watchers;
        private readonly HubConnection _hubConnection;

        public ApplicationStateManager(HubConnection hubConnection)
        {
            this._watchers = new ConcurrentDictionary<string, FileSystemWatcher>();
            this._hubConnection = hubConnection;

            this._hubConnection.On<string, string>("StartWatching", StartWatching);
            this._hubConnection.On<string>("PauseWatching", PauseWatching);
            this._hubConnection.On<string>("ResumeWatching", ResumeWatching);
            this._hubConnection.On<string>("StopWatching", StopWatching);
        }

        public void StartWatching(string path, string filter = "*.*")
        {
            if (!this._watchers.ContainsKey(path))
            {
                var watcher = new FileSystemWatcher
                {
                    Path = path,
                    Filter = filter,
                    NotifyFilter = NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName,
                };

                if (this._watchers.TryAdd(path, watcher))
                {
                    watcher.Created += OnChanged;
                    watcher.Changed += OnChanged;
                    watcher.Deleted += OnChanged;
                    watcher.Renamed += OnRenamed;

                    watcher.EnableRaisingEvents = true;
                    Log.Information($"Started watching path: {path} with filter: {filter}");
                }
            }
        }

        public void PauseWatching(string path)
        {
            if (this._watchers.TryGetValue(path, out var watcher))
            {
                watcher.EnableRaisingEvents = false;
                Log.Information($"Paused watching path: {path}");
            }
        }

        public void ResumeWatching(string path)
        {
            if (this._watchers.TryGetValue(path, out var watcher))
            {
                watcher.EnableRaisingEvents = true;
                Log.Information($"Resumed watching path: {path}");
            }
        }

        public void StopWatching(string path)
        {
            if (this._watchers.TryRemove(path, out var watcher))
            {
                watcher.Dispose();
                Log.Information($"Stopped watching path: {path}");
            }
        }

        public IEnumerable<string> GetActivePaths()
        {
            return this._watchers.Keys;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            Log.Information($"File: {e.FullPath} {e.ChangeType}");
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            Log.Information($"File: {e.OldFullPath} renamed to {e.FullPath}");
        }
    }
}