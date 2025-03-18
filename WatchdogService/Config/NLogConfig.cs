using NLog;
using NLog.Config;
using NLog.Targets;

namespace WatchdogService.Config
{
    public static class NLogConfig
    {
        public static void ConfigureNLog()
        {
            var config = new LoggingConfiguration();

            var fileTarget = new FileTarget("logfile")
            {
                FileName = "logs/watchdog-${shortdate}.log",
                Layout = "${longdate} | ${level:uppercase=true} | ${logger} | ${message} ${exception:format=tostring}",
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
                MaxArchiveFiles = 7
            };

            var consoleTarget = new ConsoleTarget("console")
            {
                Layout = "${level}: ${message}"
            };

            config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, consoleTarget);

            LogManager.Configuration = config;
        }

        public static Logger Log => LogManager.GetLogger("WatchdogService");
    }
}