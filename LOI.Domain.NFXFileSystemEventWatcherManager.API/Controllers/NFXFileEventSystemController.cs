namespace LOI.Domain.NFXFileSystemEventWatcherManager.API.Controllers
{
    using LOI.Domain.NFXFileSystemEventWatcherManager.API.Data;
    using LOI.Domain.NFXFileSystemEventWatcherManager.API.Data.Entities;
    using LOI.Domain.NFXFileSystemEventWatcherManager.API.Hubs;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.FileSystemGlobbing.Internal;

    using System;
    using System.Collections.Generic;
    using System.Linq;

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
            return this._context.NFXFileSystemWorkers.ToList();
        }

        // Add a folder to a specific machine's watch group
        [HttpPost("workers/{machineName}/watchers")]
        public async Task<ActionResult> AddWatcherForWorker(string machineName, WatchFolder watchFolder)
        {
            if (watchFolder == null || machineName != watchFolder.Machine)
            {
                return BadRequest();
            }

            watchFolder.IsActive = true; // Set IsActive to true when adding a watcher

            this._context.WatchFolders.Add(watchFolder);
            this._context.SaveChanges();

            // Notify the worker to add the watcher
            await _hubContext.Clients.Group(machineName).SendAsync("AddWatcher", watchFolder.Path, watchFolder.Filter);

            return CreatedAtAction(nameof(GetWorkers), new { machineName = machineName }, watchFolder);
        }

        // Remove a folder from a machine's watch group
        [HttpDelete("workers/{machineName}/watchers/{watcherId}")]
        public ActionResult DeleteWatcherForWorker(string machineName, int watcherId)
        {
            var existingWatcher = this._context.WatchFolders.FirstOrDefault(f => f.Id == watcherId && f.Machine == machineName);
            if (existingWatcher == null)
            {
                return NotFound();
            }

            // Notify the worker to delete the watcher
            this._hubContext.Clients.Group(machineName).SendAsync("DeleteWatcher", existingWatcher.Path);

            this._context.WatchFolders.Remove(existingWatcher);
            this._context.SaveChanges();

            return NoContent();
        }

        // Pause watching a folder
        [HttpPost("workers/{machineName}/watchers/{watcherId}/pause")]
        public ActionResult PauseWatchingFolder(string machineName, int watcherId)
        {
            var existingWatcher = this._context.WatchFolders.FirstOrDefault(f => f.Id == watcherId && f.Machine == machineName);

            if (existingWatcher == null)
            {
                return NotFound();
            }

            existingWatcher.IsActive = false;
            this._context.SaveChanges();

            // Notify the worker to stop watching
            this._hubContext.Clients.Group(machineName).SendAsync("StopWatcher", existingWatcher.Path);

            return NoContent();
        }

        // Resume watching a folder
        [HttpPost("workers/{machineName}/watchers/{watcherId}/resume")]
        public ActionResult ResumeWatchingFolder(string machineName, int watcherId)
        {
            var existingWatcher = this._context.WatchFolders.FirstOrDefault(f => f.Id == watcherId && f.Machine == machineName);
            if (existingWatcher == null)
            {
                return NotFound();
            }

            existingWatcher.IsActive = true;
            this._context.SaveChanges();

            // Notify the worker to start watching
            this._hubContext.Clients.Group(machineName).SendAsync("StartWatcher", existingWatcher.Path);

            return NoContent();
        }
    }
}
