namespace LOI.Service.NFXFileSystemEventWatcher
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using LOI.Service.NFXFileSystemEventWatcher.Models;
    using Microsoft.AspNetCore.SignalR.Client;

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly AppStateManager _appStateManager;
        private HubConnection _hubConnection;
        private readonly TimeSpan _checkInInterval = TimeSpan.FromMinutes(5);

        public Worker(ILogger<Worker> logger, AppStateManager appStateManager)
        {
            this._logger = logger;
            this._appStateManager = appStateManager;
            this._hubConnection = new HubConnectionBuilder()
                .WithUrl($"https://localhost:7188/workerHub")
                .Build();

            InitializeHubMethods();
        }

        private void InitializeHubMethods()
        {
            this._hubConnection.On<string, string>("AddWatcher", (path, filter) =>
            {
                this._appStateManager.AddWatcher(path, filter);
            });

            this._hubConnection.On<string>("StartWatcher", (path) =>
            {
                this._appStateManager.StartWatcher(path);
            });

            this._hubConnection.On<string>("StopWatcher", (path) =>
            {
                this._appStateManager.StopWatcher(path);
            });

            this._hubConnection.On<string>("DeleteWatcher", (path) =>
            {
                this._appStateManager.DeleteWatcher(path);
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => OnStopping());

            await ConnectAndInitializeFromAPI();

            while (!stoppingToken.IsCancellationRequested)
            {
                this._logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(_checkInInterval, stoppingToken);

                await CheckInWithAPI();
            }
        }

        private async Task ConnectAndInitializeFromAPI()
        {
            if (this._hubConnection.State != HubConnectionState.Connected)
            {
                await this._hubConnection.StartAsync();
                await this._hubConnection.SendAsync("InitializeWorker", Environment.MachineName);
            }
        }

        private async Task CheckInWithAPI()
        {
            if (this._hubConnection.State == HubConnectionState.Connected)
            {
                await this._hubConnection.SendAsync("CheckIn", Environment.MachineName);
            }
        }

        private async void OnStopping()
        {
            if (this._hubConnection != null && this._hubConnection.State == HubConnectionState.Connected)
            {
                await this._hubConnection.StopAsync();
                await this._hubConnection.DisposeAsync();
            }
        }
    }
}
