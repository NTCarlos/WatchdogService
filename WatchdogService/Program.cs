using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using WatchdogService.Config;
using WatchdogService.Services;

namespace WatchdogService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            NLogConfig.ConfigureNLog();

            try
            {
                NLogConfig.Log.Info("Starting Watchdog service...");

                Host.CreateDefaultBuilder(args)
                    .UseWindowsService()
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.SetBasePath(AppContext.BaseDirectory);
                        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    })
                    .ConfigureServices((context, services) =>
                    {
                        services.Configure<WatchdogConfig>(context.Configuration.GetSection("WatchdogConfig"));
                        services.AddHostedService<WatchdogBackgroundService>();

                        services.AddLogging(builder =>
                        {
                            builder.ClearProviders();
                            builder.AddNLog();
                        });
                    })
                    .Build()
                    .Run();
            }
            catch (Exception ex)
            {
                NLogConfig.Log.Fatal(ex, "There was a problem starting the Watchdog service");
            }
            finally
            {
                NLogConfig.Log.Info("Shutting down Watchdog service...");
                LogManager.Shutdown();
            }
        }
    }
}