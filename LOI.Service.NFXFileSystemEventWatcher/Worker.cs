namespace LOI.Service.NFXFileSystemEventWatcher
{
    using Microsoft.AspNetCore.SignalR.Client;
    using Serilog;

    using System.Threading;

    public class Worker : BackgroundService
    {
        private readonly string _machineName;
        private readonly HubConnection _hubConnection;
        private readonly ApplicationStateManager _appStateManager;

        public Worker(ApplicationStateManager appStateManager, HubConnection hubConnection)
        {
            this._machineName = Environment.MachineName;

            this._hubConnection = hubConnection;

            this._appStateManager = appStateManager;

            this._hubConnection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await this._hubConnection.StartAsync();
                Log.Information("Reconnected to SignalR hub after connection closed.");
                Log.Information("Started SignalR hub connection.");
            };
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await this._hubConnection.StartAsync();
            Log.Information("Reconnected to SignalR hub after connection closed.");
            Log.Information("Started SignalR hub connection.");

            await this._hubConnection.InvokeAsync("InitializeWorker", _machineName);
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(10000, cancellationToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await this._hubConnection.DisposeAsync();
            Log.Information("Disposed SignalR hub connection.");
        }
    }
}