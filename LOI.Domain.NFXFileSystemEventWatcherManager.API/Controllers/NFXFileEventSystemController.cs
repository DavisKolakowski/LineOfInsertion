namespace LOI.Domain.NFXFileSystemEventWatcherManager.API.Controllers
{
    using LOI.Domain.NFXFileSystemEventWatcherManager.API.Data.Entities;
    using LOI.Domain.NFXFileSystemEventWatcherManager.API.Data;
    using LOI.Domain.NFXFileSystemEventWatcherManager.API.Hubs;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using LOI.Domain.NFXFileSystemEventWatcherManager.API.Models;

    [ApiController]
    [Route("api/[controller]")]
    public class NFXFileEventSystemController : ControllerBase
    {
        private readonly LineOfInsertionDbContext _context;
        private readonly IHubContext<WorkerHub> _hubContext;

        public NFXFileEventSystemController(LineOfInsertionDbContext context, IHubContext<WorkerHub> hubContext)
        {
            this._context = context;
            this._hubContext = hubContext;
        }

        // Get all active workers
        [HttpGet("workers")]
        public ActionResult<IEnumerable<NFXFileSystemWorker>> GetWorkers()
        {
            return Ok(this._context.NFXFileSystemWorkers.ToList());
        }

        // Add a folder to a specific machine's watch group
        [HttpPost("workers/{machineName}/watchers")]
        public async Task<ActionResult> AddWatcherForWorker(string machineName, FolderToWatchDTO watchFolderDto)
        {
            if (machineName == null || watchFolderDto == null)
            {
                return BadRequest("Invalid request parameters.");
            }

            var watchFolder = new WatchFolder
            {
                Machine = machineName,
                Path = watchFolderDto.Path,
                Filter = watchFolderDto.Filter,
                IsActive = true,
                DateAddedUTC = DateTime.UtcNow
            };

            this._context.WatchFolders.Add(watchFolder);
            await _context.SaveChangesAsync();

            await this._hubContext.Clients.Client(machineName).SendAsync("AddWatchFolder", watchFolder.Path, watchFolder.Filter);

            return NoContent();
        }

        // Remove a folder from a machine's watch group
        [HttpDelete("workers/{machineName}/watchers/{watchPath}")]
        public async Task<ActionResult> DeleteWatcherForWorker(string machineName, string watchPath)
        {
            await this._hubContext.Clients.Client(machineName).SendAsync("RemoveWatchFolder", machineName, watchPath);
            return NoContent();
        }

        // Pause watching a folder
        [HttpPost("workers/{machineName}/watchers/{watchPath}/pause")]
        public async Task<ActionResult> PauseWatchingFolder(string machineName, string watchPath)
        {
            await this._hubContext.Clients.Client(machineName).SendAsync("PauseWatchFolder", machineName, watchPath);
            return NoContent();
        }

        // Resume watching a folder
        [HttpPost("workers/{machineName}/watchers/{watchPath}/resume")]
        public async Task<ActionResult> ResumeWatchingFolder(string machineName, string watchPath)
        {
            await this._hubContext.Clients.Client(machineName).SendAsync("ResumeWatchFolder", machineName, watchPath);
            return NoContent();
        }
    }
}