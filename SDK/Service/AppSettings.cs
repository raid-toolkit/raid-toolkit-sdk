namespace Raid.Service
{
    public class AppSettings
    {
        public string MemoryLogging { get; set; }
        public string StorageLocation { get; set; }
        public string PublicServer { get; set; }
        public ProcessWatcherSettings ProcessWatcher { get; set; }
        public UpdateSettings UpdateManager { get; set; }
    }
    public class UpdateSettings
    {
        public bool Enabled { get; set; }
        public int PollIntervalMs { get; set; }
    }
    public class ProcessWatcherSettings
    {
        public string ProcessName { get; set; }
        public int PollIntervalMs { get; set; }
    }
}