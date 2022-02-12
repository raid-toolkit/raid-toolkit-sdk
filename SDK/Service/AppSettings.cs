namespace Raid.Service
{
    public class AppSettings
    {
        public string MemoryLogging { get; set; }
        public string StorageLocation { get; set; }
        public string PublicServer { get; set; }
        public ProcessWatcherSettings ProcessWatcher { get; set; }
        public ApplicationUpdateSettings ApplicationUpdates { get; set; }
        public DataUpdateSettings DataSettings { get; set; }
        public FrameRateSettings FrameRate { get; set; }
    }
    public class DataUpdateSettings
    {
        public int IdleIntervalMs { get; set; }
        public int ActiveIntervalMs { get; set; }
        public int ActiveCooldownMs { get; set; }
    }
    public class ApplicationUpdateSettings
    {
        public int PollIntervalMs { get; set; }
    }
    public class ProcessWatcherSettings
    {
        public string ProcessName { get; set; }
        public int PollIntervalMs { get; set; }
    }
    public class FrameRateSettings
    {
        public int MaxFrameRate { get; set; }
        public int ArtifactUpgradeFrameRate { get; set; }
        public bool AutosetFramerate { get; set; }
    }
}
