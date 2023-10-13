namespace LOI.Service.NFXFileSystemEventWatcher
{
    using Microsoft.AspNetCore.SignalR.Client;
    using System.Threading;

    public class Worker : BackgroundService
    {
        private readonly string _machineName;
        private readonly HubConnection _hubConnection;
        private readonly AppStateManager _appStateManager;

        public Worker()
        {
            this._machineName = Environment.MachineName;

            this._hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7188/workerHub")
                .Build();

            this._appStateManager = new AppStateManager(this._hubConnection);

            this._hubConnection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await this._hubConnection.StartAsync();
            };
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await this._hubConnection.StartAsync();

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
        }

    }
}