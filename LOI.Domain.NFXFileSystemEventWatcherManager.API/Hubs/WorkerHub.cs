namespace LOI.Domain.NFXFileSystemEventWatcherManager.API.Hubs
{
    using LOI.Domain.NFXFileSystemEventWatcherManager.API.Data;
    using LOI.Domain.NFXFileSystemEventWatcherManager.API.Data.Entities;

    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.FileSystemGlobbing.Internal;

    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class WorkerHub : Hub
    {
        private readonly LineOfInsertionDbContext _context;

        public WorkerHub(LineOfInsertionDbContext context)
        {
            this._context = context;
        }

        public async Task InitializeWorker(string machineName)
        {
            var worker = this._context.NFXFileSystemWorkers.FirstOrDefault(w => w.Machine == machineName);
            if (worker == null)
            {
                worker = new NFXFileSystemWorker
                {
                    Machine = machineName,
                    IsConnected = true,
                    LastUpdateUTC = DateTime.UtcNow
                };
                this._context.NFXFileSystemWorkers.Add(worker);
                this._context.SaveChanges();
            }
            else
            {
                worker.LastUpdateUTC = DateTime.UtcNow;
                worker.IsConnected = true;
                this._context.SaveChanges();
            }

            // Send the initial state to the worker
            var watchFolders = this._context.WatchFolders.Where(f => f.Machine == machineName).ToList();
            await Clients.Caller.SendAsync("InitializeState", watchFolders);
        }

        public async Task CheckIn(string machineName)
        {
            var worker = _context.NFXFileSystemWorkers.FirstOrDefault(w => w.Machine == machineName);
            if (worker != null)
            {
                worker.LastUpdateUTC = DateTime.UtcNow;
                worker.IsConnected = true;
                await this._context.SaveChangesAsync();
            }
        }

        public void StartWatching(string machineName, string path)
        {
            Clients.Group(machineName).SendAsync("StartWatcher", path);
        }

        public void StopWatching(string machineName, string path)
        {
            Clients.Group(machineName).SendAsync("StopWatcher", path);
        }

        public void DeleteWatcher(string machineName, string path)
        {
            Clients.Group(machineName).SendAsync("DeleteWatcher", path);
        }
    }
}
