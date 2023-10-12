
namespace LOI.Service.NFXFileSystemEventWatcher.Models
{
    using System;
    using System.Collections.Concurrent;

    public class AppState
    {
        public ConcurrentBag<WatcherState> Watchers { get; set; } = new ConcurrentBag<WatcherState>();
    }
}
