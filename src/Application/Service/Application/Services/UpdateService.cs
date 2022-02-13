using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GitHub;
using GitHub.Schema;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Raid.Service
{
    public class UpdateService : BackgroundService
    {
        private readonly ILogger<UpdateService> Logger;
        private readonly Updater Updater;
        private readonly TimeSpan PollInterval;
        private readonly bool Enabled;
        private Release PendingRelease;

        public event EventHandler<UpdateAvailbleEventArgs> UpdateAvailable;

        public UpdateService(ILogger<UpdateService> logger, IOptions<AppSettings> appSettings, Updater updater)
        {
            Logger = logger;
            Updater = updater;
            Enabled = AppConfiguration.AutomaticallyCheckForUpdates;
            if (appSettings.Value.ApplicationUpdates != null)
            {
                PollInterval = TimeSpan.FromMilliseconds(appSettings.Value.ApplicationUpdates.PollIntervalMs);
            }
            else
            {
                PollInterval = new TimeSpan(0, 15, 0);
            }
        }

        public async Task InstallRelease(Release release)
        {
            try
            {
                Stream newRelease = await Updater.DownloadRelease(release);
                string tempDownload = Path.Join(AppConfiguration.InstallationPath, $"{AppConfiguration.ExecutableName}.update");
                string currentBackup = Path.Join(AppConfiguration.InstallationPath, $"{Path.GetFileNameWithoutExtension(AppConfiguration.ExecutableName)}.{ThisAssembly.AssemblyFileVersion}.exe");
                using (Stream newFile = File.Create(tempDownload))
                {
                    newRelease.CopyTo(newFile);
                }

                if (File.Exists(currentBackup))
                    File.Delete(currentBackup);

                File.Move(AppConfiguration.ExecutablePath, currentBackup);
                File.Move(tempDownload, AppConfiguration.ExecutablePath);
            }
            catch (Exception ex)
            {
                Logger.LogError(ServiceError.MissingUpdateAsset.EventId(), ex, $"Failed to update to {release.TagName}");
                throw;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (Enabled)
                    {
                        await CheckForUpdates();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ServiceError.UnhandledException.EventId(), ex, "Unhandled exception");
                }

                // ensure delay is included if an exception is thrown above
                try
                {
                    await Task.Delay((int)PollInterval.TotalMilliseconds, stoppingToken);
                }
                catch (OperationCanceledException) // expected if the service is shutting down
                { }
            }
        }

        public async Task<bool> CheckForUpdates()
        {
            Release release = await Updater.GetLatestRelease();
            if (!Version.TryParse(release.TagName.TrimStart('v').Split('-')[0], out Version releaseVersion))
                return false;

            if (releaseVersion > AppConfiguration.AppVersion)
            {
                if (PendingRelease?.TagName != release.TagName)
                {
                    PendingRelease = release;
                    UpdateAvailable?.Invoke(this, new(release));
                    return true;
                }
            }
            return false;
        }

        public class UpdateAvailbleEventArgs : EventArgs
        {
            public Release Release { get; private set; }
            public UpdateAvailbleEventArgs(Release release) => Release = release;
        }
    }
}
