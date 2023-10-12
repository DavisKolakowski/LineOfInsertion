namespace LOI.Domain.NFXFileSystemEventWatcherManager.API.Data
{
    using LOI.Domain.NFXFileSystemEventWatcherManager.API.Data.Entities;

    using Microsoft.EntityFrameworkCore;

    using System.Collections.Generic;

    public class LineOfInsertionDbContext : DbContext
    {
        public LineOfInsertionDbContext(DbContextOptions<LineOfInsertionDbContext> options) : base(options) { }

        public DbSet<NFXFileSystemWorker> NFXFileSystemWorkers { get; set; }
        public DbSet<WatchFolder> WatchFolders { get; set; }
    }
}
