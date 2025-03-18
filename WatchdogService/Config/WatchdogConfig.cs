namespace WatchdogService.Config
{
    public class WatchdogConfig
    {
        public required string AppPath { get; set; }
        public required string AppName { get; set; }
        public int CheckInterval { get; set; } = 30000;
    }
}
