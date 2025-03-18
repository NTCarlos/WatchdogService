using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using WatchdogService.Config;

namespace WatchdogService.Services
{
    public class WatchdogBackgroundService : BackgroundService
    {
        private readonly WatchdogConfig _config;
        private readonly ILogger<WatchdogBackgroundService> _logger;

        public WatchdogBackgroundService(IOptions<WatchdogConfig> config, ILogger<WatchdogBackgroundService> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Watchdog Service Started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                MonitorProcess();
                await Task.Delay(_config.CheckInterval, stoppingToken);
            }
        }

        private void MonitorProcess()
        {
            Process[] processes = Process.GetProcessesByName(_config.AppName);

            if (processes.Length == 0)
            {
                _logger.LogWarning($"{DateTime.Now}: Process not running. Restarting...");
                StartProcess();
            }
            else
            {
                foreach (var process in processes)
                {
                    if (!process.Responding)
                    {
                        _logger.LogError($"{DateTime.Now}: Process frozen. Restarting...");
                        RestartProcess(process);
                    }
                }
            }
        }

        private void StartProcess()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(_config.AppPath)
                {
                    UseShellExecute = true,
                    Verb = "runas", // Run with admin rights
                    WorkingDirectory = Path.GetDirectoryName(_config.AppPath)
                };
                Process.Start(psi);
                _logger.LogInformation($"{DateTime.Now}: Process restarted.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error starting process: {ex.Message}");
            }
        }


        private void RestartProcess(Process existingProcess)
        {
            try
            {
                existingProcess.Kill();
                existingProcess.WaitForExit();
                StartProcess();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error restarting process: {ex.Message}");
            }
        }
    }
}
