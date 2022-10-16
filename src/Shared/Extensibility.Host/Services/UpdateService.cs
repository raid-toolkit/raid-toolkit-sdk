using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GitHub;
using GitHub.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raid.Toolkit.Common;

namespace Raid.Toolkit.Extensibility.Host.Services
{
    public class UpdateSettings
    {
        public int PollIntervalMs { get; set; }
    }
    public class UpdateService : PollingBackgroundService
    {
        public class UpdateAvailbleEventArgs : EventArgs
        {
            public Release Release { get; private set; }
            public UpdateAvailbleEventArgs(Release release)
            {
                Release = release;
            }
        }

        private readonly Updater Updater;
        private readonly bool Enabled;
        private readonly Version CurrentVersion;
        private Release PendingRelease;
        private readonly TimeSpan _PollInterval;
        private protected override TimeSpan PollInterval => _PollInterval;

        public event EventHandler<UpdateAvailbleEventArgs> UpdateAvailable;

        public UpdateService(ILogger<UpdateService> logger, IOptions<UpdateSettings> settings, Updater updater)
            : base(logger)
        {
            CurrentVersion = Version.Parse(FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).FileVersion);
            Updater = updater;
            Enabled = RegistrySettings.AutomaticallyCheckForUpdates;
            _PollInterval = settings.Value.PollIntervalMs > 0 ? TimeSpan.FromMilliseconds(settings.Value.PollIntervalMs) : new TimeSpan(0, 15, 0);
        }

        public async Task InstallRelease(Release release)
        {
            try
            {
                Stream newRelease = await Updater.DownloadSetup(release, null);
                string tempDownload = Path.Combine(RegistrySettings.InstallationPath, $"RaidToolkitSetup.{release.TagName}.exe");
                if (File.Exists(tempDownload))
                    File.Delete(tempDownload);

                using (Stream newFile = File.Create(tempDownload))
                {
                    newRelease.CopyTo(newFile);
                }

                Process.Start(tempDownload, "/install LaunchAfterInstallation=1");
            }
            catch (Exception ex)
            {
                Logger.LogError(ServiceError.MissingUpdateAsset.EventId(), ex, $"Failed to update to {release.TagName}");
                throw;
            }
        }

        protected override async Task ExecuteOnceAsync(CancellationToken token)
        {
            try
            {
                if (Enabled)
                {
                    _ = await CheckForUpdates();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ServiceError.UnhandledException.EventId(), ex, "Unhandled exception");
            }
        }

        public async Task<bool> CheckForUpdates(bool force = false)
        {
            Release release = await Updater.GetLatestRelease();
            if (!Version.TryParse(release.TagName.TrimStart('v').Split('-')[0], out Version releaseVersion))
                return false;

            if (releaseVersion > CurrentVersion)
            {
                if (force || PendingRelease?.TagName != release.TagName)
                {
                    PendingRelease = release;
                    UpdateAvailable?.Raise(this, new(release));
                    return true;
                }
            }
            return false;
        }

    }
}
