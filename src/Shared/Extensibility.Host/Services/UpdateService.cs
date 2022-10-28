using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GitHub;
using GitHub.Schema;
using Il2CppToolkit.Common.Errors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility.Notifications;

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
        private readonly INotificationSink Notify;
        private readonly bool Enabled;
        private readonly Version CurrentVersion;
        private Release? PendingRelease;
        private readonly TimeSpan _PollInterval;
        private protected override TimeSpan PollInterval => _PollInterval;

        public event EventHandler<UpdateAvailbleEventArgs>? UpdateAvailable;

        public UpdateService(
            ILogger<UpdateService> logger,
            IOptions<UpdateSettings> settings,
            Updater updater,
            INotificationManager notificationManager)
            : base(logger)
        {
            string? fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location).FileVersion;
            ErrorHandler.VerifyElseThrow(fileVersion != null, ServiceError.UnhandledException, "Missing version information");
            CurrentVersion = Version.Parse(fileVersion!);
            Updater = updater;
            Enabled = RegistrySettings.AutomaticallyCheckForUpdates;
            _PollInterval = settings.Value.PollIntervalMs > 0 ? TimeSpan.FromMilliseconds(settings.Value.PollIntervalMs) : new TimeSpan(0, 15, 0);

            Notify = notificationManager.RegisterHandler("update");
            Notify.Activated += Notify_Activated;
        }

        private void Notify_Activated(object? sender, NotificationActivationEventArgs e)
        {
            if (e.Arguments.TryGetValue("action", out string? action) && action == "install-update")
            {

            }
            // throw new NotImplementedException();
        }

        public Task InstallUpdate()
        {
            return PendingRelease == null ? Task.CompletedTask : InstallRelease(PendingRelease);
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

                _ = Process.Start(tempDownload, "/install LaunchAfterInstallation=1");
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
                    _ = await CheckForUpdates(false, false);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ServiceError.UnhandledException.EventId(), ex, "Unhandled exception");
            }
        }

        public async Task<bool> CheckForUpdates(bool userRequested, bool force)
        {
            Release release = await Updater.GetLatestRelease();
            if (!Version.TryParse(release.TagName.TrimStart('v').Split('-')[0], out Version? releaseVersion))
                return false;

            if (releaseVersion > CurrentVersion)
            {
                if (force || PendingRelease?.TagName != release.TagName)
                {
                    PendingRelease = release;
                    UpdateAvailable?.Raise(this, new(release));
                    ToastNotification notification = new(
                        "Update available",
                        $"A new version has been released!\n{release.TagName} is now available for install. Click here to install and update!",
                        "install-update"
                        );
                    Notify.SendNotification(notification);
                    return true;
                }
            }
            else if (userRequested)
            {
                ToastNotification notification = new("No updates", "You are already running the latest version!", "none");
                Notify.SendNotification(notification);

            }
            return false;
        }

    }
}
