using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ApplicationWatcher.Service.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", $"ApplicationWatcher_Log.txt"),
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u3}] [{SourceContext}] ({MachineName}/{ThreadId}) {Message}{NewLine}{Exception}")
                .CreateLogger();

            try
            {
                Log.Information("Starting up the service");

                CreateHostBuilder(args).Build().Run();
                return;
            }
            catch (Exception e)
            {
                Log.Fatal(e, "There was a problem starting the service");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog()
                .UseWindowsService();
    }
}
