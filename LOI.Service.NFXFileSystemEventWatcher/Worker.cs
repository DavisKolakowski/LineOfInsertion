namespace LOI.Service.NFXFileSystemEventWatcher
{
    using Microsoft.AspNetCore.SignalR.Client;
    using Serilog;

    using System.Threading;

    public class Worker : BackgroundService
    {       
        private readonly ILogger<Worker> _logger;
        private readonly HubConnection _hubConnection;
        private readonly WatchServiceManager _watchService;

        public Worker(ILogger<Worker> logger, HubConnection hubConnection, WatchServiceManager watchService)
        {
            this._logger = logger;
            this._hubConnection = hubConnection;
            this._watchService = watchService;

            this._hubConnection.On<string, string?>("AddWatcher", this._watchService.AddWatcher);
            this._hubConnection.On<string>("DisableWatcher", this._watchService.DisableWatcher);
            this._hubConnection.On<string>("EnableWatcher", this._watchService.EnableWatcher);
            this._hubConnection.On<string>("RemoveWatcher", this._watchService.RemoveWatcher);
        }

        private readonly string MachineName = Environment.MachineName;

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            if (this._hubConnection.State == HubConnectionState.Disconnected)
            {
                await this._hubConnection.StartAsync();
                this._logger.LogInformation("Started SignalR hub connection.");
            }

            await this._hubConnection.InvokeAsync("InitializeWorker", this.MachineName);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                this._logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await this._hubConnection.InvokeAsync("Heartbeat", this.MachineName);
                await Task.Delay(1000, stoppingToken);
            }
        }    

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await this._hubConnection.DisposeAsync();
            this._logger.LogInformation("Disposed SignalR hub connection.");
        }
    }
}