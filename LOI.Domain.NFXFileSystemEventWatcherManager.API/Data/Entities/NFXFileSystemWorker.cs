namespace LOI.Domain.NFXFileSystemEventWatcherManager.API.Data.Entities
{
    using Microsoft.AspNetCore.SignalR;

    public class NFXFileSystemWorker
    {
        public int Id { get; set; }
        public string Machine { get; set; }
        public bool IsConnected { get; set; }
        public DateTime LastUpdateUTC { get; set; }

        public List<WatchFolder> WatchFolders { get; set; }
    }
}
