namespace LOI.Service.NFXFileSystemEventWatcher
{
    using Microsoft.AspNetCore.SignalR.Client;
    using System.Collections.Concurrent;

    public class AppStateManager
    {
        private readonly ConcurrentDictionary<string, NFXFileSystemWatcher> _watchers;
        private readonly HubConnection _hubConnection;

        public AppStateManager(HubConnection hubConnection)
        {
            this._watchers = new ConcurrentDictionary<string, NFXFileSystemWatcher>();
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
                var watcher = new NFXFileSystemWatcher(path, filter);
                if (this._watchers.TryAdd(path, watcher))
                {
                    watcher.Start();
                }
            }
        }

        public void PauseWatching(string path)
        {
            if (this._watchers.TryGetValue(path, out var watcher))
            {
                watcher.Pause();
            }
        }

        public void ResumeWatching(string path)
        {
            if (this._watchers.TryGetValue(path, out var watcher))
            {
                watcher.Resume();
            }
        }

        public void StopWatching(string path)
        {
            if (this._watchers.TryRemove(path, out var watcher))
            {
                watcher.Dispose();
            }
        }

        public IEnumerable<string> GetActivePaths()
        {
            return this._watchers.Keys;
        }
    }
}