namespace LOI.Domain.NFXFileSystemEventWatcherManager.API.Hubs
{
    using LOI.Domain.NFXFileSystemEventWatcherManager.API.Data.Entities;
    using LOI.Domain.NFXFileSystemEventWatcherManager.API.Data;
    using Microsoft.AspNetCore.SignalR;

    public class WorkerHub : Hub
    {
        private readonly ILogger<WorkerHub> _logger;
        private readonly LineOfInsertionDbContext _context;

        public WorkerHub(ILogger<WorkerHub> logger, LineOfInsertionDbContext context)
        {
            this._logger = logger;
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

            var watchFolders = this._context.WatchFolders.Where(f => f.Machine == machineName).ToList();

            foreach (var watchFolder in watchFolders)
            {
                await Clients.Caller.SendAsync("AddWatcher", watchFolder.Path, watchFolder.Filter);
                if (watchFolder.IsActive)
                {
                    await Clients.Caller.SendAsync("EnableWatcher", watchFolder.Path);
                }
            }
        }

        public async Task Heartbeat(string machineName)
        {
            var worker = this._context.NFXFileSystemWorkers.FirstOrDefault(w => w.Machine == machineName);
            if (worker != null)
            {
                worker.LastUpdateUTC = DateTime.UtcNow;
                worker.IsConnected = true;
                await this._context.SaveChangesAsync();
            }
        }

        public async Task AddWatchFolder(string machineName, string path, string? filter)
        {
            var machine = this._context.NFXFileSystemWorkers.FirstOrDefault(w => w.Machine == machineName);
            var watchFolder = this._context.WatchFolders.FirstOrDefault(wf => wf.Machine == machineName && wf.Path == path);

            if (machine!.Connection != null)
            {
                if (watchFolder == null)
                {
                    await Clients.Client(machine.Connection).SendAsync("AddWatcher", path, filter);
                    await Clients.Client(machine.Connection).SendAsync("EnableWatcher", path);

                    var newWatchFolder = new WatchFolder
                    {
                        Machine = machineName,
                        Path = path,
                        IsActive = true
                    };
                    this._context.WatchFolders.Add(newWatchFolder);

                    await this._context.SaveChangesAsync();
                }
                else if (!watchFolder.IsActive)
                {
                    watchFolder.IsActive = true;
                    this._context.Update(watchFolder);
                    await this._context.SaveChangesAsync();

                    await Clients.Client(machine.Connection).SendAsync("EnableWatcher", path);
                }
                else
                {
                    this._logger.LogInformation("A WatchFolder with the specified connection and path already exists.");
                }
            }
        }

        public async Task PauseWatchFolder(string machineName, string path)
        {
            var machine = this._context.NFXFileSystemWorkers.FirstOrDefault(w => w.Machine == machineName);
            var watchFolder = this._context.WatchFolders.FirstOrDefault(wf => wf.Machine == machineName && wf.Path == path);

            if (machine!.Connection != null)
            {
                if (watchFolder != null && watchFolder.IsActive)
                {
                    await Clients.Client(machine.Connection).SendAsync("DisableWatcher", path);

                    watchFolder.IsActive = false;
                    this._context.Update(watchFolder);
                    await this._context.SaveChangesAsync();
                }
                else
                {
                    this._logger.LogInformation("A WatchFolder with the specified machine name and path either does not exist or is already disabled.");
                }
            }
        }

        public async Task ResumeWatchFolder(string machineName, string path)
        {
            var machine = this._context.NFXFileSystemWorkers.FirstOrDefault(w => w.Machine == machineName);
            var watchFolder = _context.WatchFolders.FirstOrDefault(wf => wf.Machine == machineName && wf.Path == path);

            if (machine!.Connection != null)
            {
                if (watchFolder != null && !watchFolder.IsActive)
                {
                    await Clients.Client(machine.Connection).SendAsync("EnableWatcher", path);

                    watchFolder.IsActive = true;
                    this._context.Update(watchFolder);
                    await this._context.SaveChangesAsync();                  
                }
                else
                {
                    this._logger.LogInformation("A WatchFolder with the specified machine name and path either does not exist or is already active.");
                }
            }
        }

        public async Task RemoveWatchFolder(string machineName, string path)
        {
            var machine = this._context.NFXFileSystemWorkers.FirstOrDefault(w => w.Machine == machineName);
            var watchFolder = this._context.WatchFolders.FirstOrDefault(wf => wf.Machine == machineName && wf.Path == path);

            if (machine!.Connection != null)
            {
                if (watchFolder != null)
                {
                    await Clients.Client(machine.Connection).SendAsync("RemoveWatcher", path);

                    this._context.WatchFolders.Remove(watchFolder);
                    await this._context.SaveChangesAsync();
                }
                else
                {
                    this._logger.LogInformation("A WatchFolder with the specified machine name and path does not exist.");
                }
            }
        }
    }
}