namespace LOI.Domain.NFXFileSystemEventWatcherManager.API.Data.Entities
{
    public class WatchFolder
    {
        public int Id { get; set; }
        public string Machine { get; set; }
        public string Filter { get; set; }
        public string Path { get; set; }
        public DateTime? LastFileEventUTC { get; set; }
        public DateTime? DateModifiedUTC { get; set; }
        public DateTime DateAddedUTC { get; set; }
        public bool IsActive { get; set; }

        public NFXFileSystemWorker NFXFileSystemWorker { get; set; }
    }
}
