namespace LOI.Service.NFXFileSystemEventWatcher
{
    using Microsoft.AspNetCore.SignalR.Client;

    using Serilog;

    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Starting Worker...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Worker terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(new HubConnectionBuilder()
                            .WithUrl("https://localhost:7188/workerHub")
                            .Build());

                    services.AddSingleton<ApplicationStateManager>();
                    services.AddHostedService<Worker>();
                });
    }
}