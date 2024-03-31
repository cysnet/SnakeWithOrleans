using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;

namespace Snake.Client
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //var invariant = "System.Data.SqlClient";

            //var connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=orleanstest;User Id=sa;Password=123;";

            using IHost host = new HostBuilder()
                .ConfigureServices(sc =>
                {
                    sc.AddSingleton<Form1>();
                })
                .UseOrleansClient(clientBuilder =>
                {
                    clientBuilder.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "my-first-cluster";
                        options.ServiceId = "MyOrleansService";
                    });
                    clientBuilder.UseLocalhostClustering();
                    //clientBuilder.UseAdoNetClustering(options =>
                    //        {
                    //            options.Invariant = invariant;
                    //            options.ConnectionString = connectionString;
                    //        });
                })
                .Build();
            host.Start();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(host.Services.GetRequiredService<Form1>());
        }
    }
}