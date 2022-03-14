using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host.Services;

namespace Raid.Toolkit
{
    public class AppSettings
    {
        public string? MemoryLogging { get; set; }
        public string? StorageLocation { get; set; }
        public ProcessManagerSettings? ProcessManager { get; set; }
        public UpdateSettings? ApplicationUpdates { get; set; }
        public DataUpdateSettings? DataSettings { get; set; }
    }
}