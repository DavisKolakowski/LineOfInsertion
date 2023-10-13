
namespace LOI.Domain.NFXFileSystemEventWatcherManager.API
{
    using LOI.Domain.NFXFileSystemEventWatcherManager.API.Data;
    using LOI.Domain.NFXFileSystemEventWatcherManager.API.Hubs;

    using Microsoft.EntityFrameworkCore;
    using Serilog.Events;
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
                Log.Information("Starting Gateway...");
                var builder = WebApplication.CreateBuilder(args);
                var connString = builder.Configuration.GetConnectionString("LineOfInsertionDbConnection");

                // Add services to the container.
                builder.Services.AddDbContext<LineOfInsertionDbContext>(options => options.UseSqlServer(connString));

                builder.Services.AddSignalR().AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                });

                builder.Services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();


                app.MapControllers();
                app.MapHub<WorkerHub>("/workerHub");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Gateway terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}