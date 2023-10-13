namespace LOI.Domain.NFXFileSystemEventWatcherManager.API.Data
{
    using LOI.Domain.NFXFileSystemEventWatcherManager.API.Data.Entities;

    using Microsoft.EntityFrameworkCore;

    public class LineOfInsertionDbContext : DbContext
    {
        public LineOfInsertionDbContext(DbContextOptions<LineOfInsertionDbContext> options) : base(options) { }

        public DbSet<NFXFileSystemWorker> NFXFileSystemWorkers { get; set; }
        public DbSet<WatchFolder> WatchFolders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NFXFileSystemWorker>()
                .HasMany(f => f.WatchFolders)
                .WithOne(w => w.NFXFileSystemWorker)
                .HasForeignKey(w => w.Machine)
                .HasPrincipalKey(f => f.Machine);
        }
    }
}
