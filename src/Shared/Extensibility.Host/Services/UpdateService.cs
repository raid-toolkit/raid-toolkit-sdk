using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.WinUI.Notifications;

using GitHub;
using GitHub.Schema;
using Il2CppToolkit.Common.Errors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility.Interfaces;
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
        private readonly IAppService AppService;
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
            IAppService appService,
            Updater updater,
            INotificationManager notificationManager)
            : base(logger)
        {
            string? fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location).FileVersion;
            ErrorHandler.VerifyElseThrow(fileVersion != null, ServiceError.UnhandledException, "Missing version information");
            CurrentVersion = Version.Parse(fileVersion!);
            Updater = updater;
            AppService = appService;
            Enabled = RegistrySettings.AutomaticallyCheckForUpdates;
            _PollInterval = settings.Value.PollIntervalMs > 0 ? TimeSpan.FromMilliseconds(settings.Value.PollIntervalMs) : new TimeSpan(0, 15, 0);

            Notify = notificationManager.RegisterHandler("update");
            Notify.Activated += Notify_Activated;
        }

        private void Notify_Activated(object? sender, NotificationActivationEventArgs e)
        {
            if (e.Arguments.TryGetValue(NotificationConstants.Action, out string? action))
            {
                switch (action)
                {
                    case "install-update":
                        {
                            InstallUpdate();
                            break;
                        }
                }
            }
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
                    newRelease.Seek(0, SeekOrigin.Begin);
                    newRelease.CopyTo(newFile);
                }

                _ = Process.Start(tempDownload, "/update LaunchAfterInstallation=1");
                AppService.Exit();
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
                    ToastContentBuilder tcb = new ToastContentBuilder()
                        .AddText("Update available")
                        .AddText($"A new version has been released!\n{release.TagName} is now available for install. Click here to install and update!")
                        .AddButton(new ToastButton("Update now", Notify.GetArguments("install-update")))
                        .AddButton(new ToastButtonSnooze("Update later"))
                        .AddButton(new ToastButtonDismiss());
                    Notify.SendNotification(tcb.Content);
                    return true;
                }
            }
            else if (userRequested)
            {
                ToastContentBuilder tcb = new ToastContentBuilder()
                    .AddText("No updates")
                    .AddText("You are already running the latest version!");
                Notify.SendNotification(tcb.Content);
            }
            return false;
        }

    }
}
