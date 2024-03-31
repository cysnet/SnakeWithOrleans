
using Orleans.Configuration;
using Snake.Common;

namespace Snake.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<SnakeHub>();

            builder.Host.UseOrleans(static siloHostBuilder =>
            {
                //var invariant = "System.Data.SqlClient";

                //var connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=orleanstest;User Id=sa;Password=123;";

                //siloHostBuilder.AddLogStorageBasedLogConsistencyProvider("LogStorage");

                // Use ADO.NET for clustering
                //siloHostBuilder.UseAdoNetClustering(options =>
                //{
                //    options.Invariant = invariant;
                //    options.ConnectionString = connectionString;
                //});

                siloHostBuilder.ConfigureLogging(logging => logging.AddConsole());

                siloHostBuilder.Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "my-first-cluster";
                    options.ServiceId = "SampleApp";
                });

                // Use ADO.NET for persistence
                //siloHostBuilder.AddAdoNetGrainStorage("snake", options =>
                //{
                //    options.Invariant = invariant;
                //    options.ConnectionString = connectionString;
                //});
                siloHostBuilder.UseLocalhostClustering();
                siloHostBuilder.AddMemoryGrainStorage("memorysnake");

                // Use ADO.NET for reminder service
                //siloHostBuilder.UseAdoNetReminderService(options =>
                //{
                //    options.Invariant = invariant;
                //    options.ConnectionString = connectionString;
                //});

                siloHostBuilder.UseDashboard(x => x.HostSelf = true);


                siloHostBuilder.AddMemoryStreams("StreamProvider").AddMemoryGrainStorage("PubSubStore");
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseAuthorization();

            app.MapGet("/", static async (IGrainFactory client) =>
            {
                return Results.Ok("ok");
            });


            app.Map("/dashboard", x => x.UseOrleansDashboard());

            app.MapHub<SnakeHub>("/ping");

            app.Run();
        }
    }
}
