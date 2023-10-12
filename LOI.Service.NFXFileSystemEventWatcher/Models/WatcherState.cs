
namespace LOI.Service.NFXFileSystemEventWatcher.Models
{
    public class WatcherState
    {
        public string Path { get; set; }
        public NFXFileSystemWatcher Watcher { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
