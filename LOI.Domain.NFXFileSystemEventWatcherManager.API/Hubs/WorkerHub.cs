namespace LOI.Domain.NFXFileSystemEventWatcherManager.API.Hubs
{
    using LOI.Domain.NFXFileSystemEventWatcherManager.API.Data.Entities;
    using LOI.Domain.NFXFileSystemEventWatcherManager.API.Data;
    using Microsoft.AspNetCore.SignalR;

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
                    Connection = Context.ConnectionId,
                    LastUpdateUTC = DateTime.UtcNow
                };
                this._context.NFXFileSystemWorkers.Add(worker);
            }
            else
            {
                worker.LastUpdateUTC = DateTime.UtcNow;
                worker.IsConnected = true;
                worker.Connection = Context.ConnectionId;
            }
            await this._context.SaveChangesAsync();

            var watchFolders = _context.WatchFolders.Where(f => f.Machine == machineName && f.IsActive).ToList();

            foreach (var watchFolder in watchFolders)
            {
                await Clients.Caller.SendAsync("StartWatching", watchFolder.Path, watchFolder.Filter);
            }
        }

        public async Task CheckIn(string machineName)
        {
            var worker = this._context.NFXFileSystemWorkers.FirstOrDefault(w => w.Machine == machineName);
            if (worker != null)
            {
                worker.LastUpdateUTC = DateTime.UtcNow;
                worker.IsConnected = true;
                await this._context.SaveChangesAsync();
            }
        }

        public async Task AddWatchFolder(string machineName, string path)
        {
            var watchFolder = this._context.WatchFolders.FirstOrDefault(wf => wf.Machine == machineName && wf.Path == path);

            if (watchFolder == null)
            {
                var newWatchFolder = new WatchFolder
                {
                    Machine = machineName,
                    Path = path,
                    IsActive = true
                };
                this._context.WatchFolders.Add(newWatchFolder);
                await this._context.SaveChangesAsync();

                await Clients.Client(machineName).SendAsync("StartWatching", path);
            }
            else if (!watchFolder.IsActive)
            {
                watchFolder.IsActive = true;
                this._context.Update(watchFolder);
                await this._context.SaveChangesAsync();

                await Clients.Client(machineName).SendAsync("ResumeWatching", path);
            }
            else
            {
                await Clients.Caller.SendAsync("Notification", "A WatchFolder with the specified machine name and path is already active.");
            }
        }

        public async Task RemoveWatchFolder(string machineName, string path)
        {
            var watchFolder = this._context.WatchFolders.FirstOrDefault(wf => wf.Machine == machineName && wf.Path == path);

            if (watchFolder != null)
            {
                this._context.WatchFolders.Remove(watchFolder);
                await this._context.SaveChangesAsync();

                await Clients.Client(machineName).SendAsync("StopWatching", path);
            }
            else
            {
                await Clients.Caller.SendAsync("Notification", "A WatchFolder with the specified machine name and path does not exist.");
            }
        }

        public async Task PauseWatchFolder(string machineName, string path)
        {
            var watchFolder = this._context.WatchFolders.FirstOrDefault(wf => wf.Machine == machineName && wf.Path == path);

            if (watchFolder != null && watchFolder.IsActive)
            {
                watchFolder.IsActive = false;
                this._context.Update(watchFolder);
                await this._context.SaveChangesAsync();

                await Clients.Client(machineName).SendAsync("PauseWatching", path);
            }
            else
            {
                await Clients.Caller.SendAsync("Notification", "A WatchFolder with the specified machine name and path either does not exist or is already paused.");
            }
        }

        public async Task ResumeWatchFolder(string machineName, string path)
        {
            var watchFolder = _context.WatchFolders.FirstOrDefault(wf => wf.Machine == machineName && wf.Path == path);

            if (watchFolder != null && !watchFolder.IsActive)
            {
                watchFolder.IsActive = true;
                this._context.Update(watchFolder);
                await this._context.SaveChangesAsync();

                await Clients.Client(machineName).SendAsync("ResumeWatching", path);
            }
            else
            {
                await Clients.Caller.SendAsync("Notification", "A WatchFolder with the specified machine name and path either does not exist or is already active.");
            }
        }
    }
}