namespace LOI.Service.NFXFileSystemEventWatcher
{
    using LOI.Service.NFXFileSystemEventWatcher.Models;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AppStateManager
    {
        private readonly List<WatcherState> _watchers = new List<WatcherState>();

        public void AddWatcher(string path, string filter)
        {
            if (!this._watchers.Any(w => w.Path == path))
            {
                var watcher = new NFXFileSystemWatcher(path, filter);
                this._watchers.Add(new WatcherState { Watcher = watcher, Path = path });
            }
        }

        public void StartWatcher(string path)
        {
            var watcherState = this._watchers.FirstOrDefault(w => w.Path == path);
            if (watcherState != null && !watcherState.IsActive)
            {
                watcherState.Watcher.Start();
                watcherState.IsActive = true;
            }
        }

        public void StopWatcher(string path)
        {
            var watcherState = this._watchers.FirstOrDefault(w => w.Path == path);
            if (watcherState != null && watcherState.IsActive)
            {
                watcherState.Watcher.Stop();
                watcherState.IsActive = false;
            }
        }

        public void DeleteWatcher(string path)
        {
            var watcherState = this._watchers.FirstOrDefault(w => w.Path == path);
            if (watcherState != null)
            {
                watcherState.Watcher.Dispose();
                this._watchers.Remove(watcherState);
            }
        }
    }
}
