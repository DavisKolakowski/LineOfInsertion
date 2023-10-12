
namespace LOI.Domain.NFXFileSystemEventWatcherManager.API
{
    using LOI.Domain.NFXFileSystemEventWatcherManager.API.Data;
    using LOI.Domain.NFXFileSystemEventWatcherManager.API.Hubs;

    using Microsoft.EntityFrameworkCore;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<LineOfInsertionDbContext>(options => options.UseSqlServer("LineOfInsertionDbConnection"));

            builder.Services.AddSignalR();

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
    }
}