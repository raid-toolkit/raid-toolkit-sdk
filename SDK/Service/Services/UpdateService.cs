using System;
using System.Diagnostics;
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
        private Release PendingRelease;

        public event EventHandler<UpdateAvailbleEventArgs> UpdateAvailable;

        public UpdateService(ILogger<UpdateService> logger, IOptions<AppSettings> appSettings, Updater updater)
        {
            Logger = logger;
            Updater = updater;
            PollInterval = appSettings.Value.UpdateManager != null
                ? TimeSpan.FromMilliseconds(appSettings.Value.UpdateManager.PollIntervalMs)
                : new TimeSpan(0, 15, 0);
        }

        public async Task InstallRelease(Release release)
        {
            try
            {
                Stream newRelease = await Updater.DownloadRelease(release);
                string tempDownload = Path.Join(AppConfiguration.ExecutableDirectory, $"{AppConfiguration.ExecutableName}.update");
                string currentBackup = Path.Join(AppConfiguration.ExecutableDirectory, $"{AppConfiguration.ExecutableName}.update");
                using (Stream newFile = File.Create(tempDownload))
                {
                    newRelease.CopyTo(newFile);
                }

                File.Move(AppConfiguration.ExecutablePath, currentBackup);
                File.Move(tempDownload, AppConfiguration.ExecutablePath);
            }
            catch (Exception ex)
            {
                Logger.LogError(ServiceError.MissingUpdateAsset.EventId(), ex, $"Failed to update to {release.TagName}");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckForUpdates();
                try
                {
                    await Task.Delay((int)PollInterval.TotalMilliseconds, stoppingToken);
                }
                catch (OperationCanceledException) // expected if the service is shutting down
                { }
            }
        }

        private async Task CheckForUpdates()
        {
            Release release = await Updater.GetLatestRelease();
            if (!Version.TryParse(release.TagName.TrimStart('v').Split('-')[0], out Version releaseVersion))
                return;

            // if (releaseVersion > AppConfiguration.AppVersion)
            {
                if (PendingRelease?.TagName != release.TagName)
                {
                    PendingRelease = release;
                    UpdateAvailable?.Invoke(this, new(release));
                }
            }
        }

        public class UpdateAvailbleEventArgs : EventArgs
        {
            public Release Release { get; private set; }
            public UpdateAvailbleEventArgs(Release release) => Release = release;
        }
    }
}