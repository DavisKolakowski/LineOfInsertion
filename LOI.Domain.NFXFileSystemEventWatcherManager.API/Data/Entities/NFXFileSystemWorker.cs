namespace LOI.Domain.NFXFileSystemEventWatcherManager.API.Data.Entities
{
    using Microsoft.AspNetCore.SignalR;

    public class NFXFileSystemWorker
    {
        public int Id { get; set; }
        public string Machine { get; set; } // Unique, principal associate key for WatchFolders Machine
        public bool IsConnected { get; set; }
        public string? Connection { get; set; } // SignalR connection id
        public DateTime LastUpdateUTC { get; set; }

        public List<WatchFolder> WatchFolders { get; set; } = new List<WatchFolder>();
    }
}
