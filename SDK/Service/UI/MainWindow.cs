using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raid.Service.UI
{
    public partial class MainWindow : Form
    {
        private GitHub.Schema.Release LatestRelease;
        private readonly UpdateService UpdateService;
        private readonly IHostApplicationLifetime Lifetime;
        private readonly MainService MainService;
        private readonly UserData UserData;
        private readonly ILogger<MainWindow> Logger;
        private readonly RunOptions RunOptions;

        public MainWindow(ILogger<MainWindow> logger, UpdateService updateService, IHostApplicationLifetime appLifetime, MainService mainService, UserData userData, RunOptions runOptions)
        {
            InitializeComponent();
            RunOptions = runOptions;
            Lifetime = appLifetime;
            Logger = logger;
            UpdateService = updateService;
            UserData = userData;
            MainService = mainService;
            UpdateService.UpdateAvailable += OnUpdateAvailable;
            appTrayIcon.Text = $"Raid Toolkit v{ThisAssembly.AssemblyFileVersion}";
            appTrayIcon.Icon = Icon.ExtractAssociatedIcon(AppConfiguration.ExecutablePath);
            appTrayIcon.Visible = true;
        }

        public bool RequestPermissions(string origin)
        {
            if (InvokeRequired)
            {
                return (bool)Invoke(new Func<bool>(() => RequestPermissions(origin)));
            }

            Logger.LogInformation(ServiceEvent.UserPermissionRequest.EventId(), $"Requesting permission for {origin}");
            UserSettings settings = UserData.ReadUserSettings();
            if (settings.AllowedOrigins.Contains(origin.ToLowerInvariant()))
            {
                Logger.LogInformation(ServiceEvent.UserPermissionCached.EventId(), $"Permission already granted for {origin}");
                return true;
            }
            using (Form form = new() { TopMost = true })
            {
                var result = MessageBox.Show(form, $"Would you like to give access to {origin}?", "Raid Toolkit", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.No)
                {
                    Logger.LogError(ServiceEvent.UserPermissionReject.EventId(), $"Permission rejected for {origin}");
                    return false;
                }
            }

            settings.AllowedOrigins.Add(origin.ToLowerInvariant());
            UserData.WriteUserSettings(settings);
            Logger.LogInformation(ServiceEvent.UserPermissionAccept.EventId(), $"Permission accepted for {origin}");
            return true;
        }

        private void OnUpdateAvailable(object sender, UpdateService.UpdateAvailbleEventArgs e)
        {
            if (LatestRelease?.TagName == e.Release.TagName)
                return; // already notified for this update

            LatestRelease = e.Release;
            appTrayIcon.ShowBalloonTip(
                10000,
                "Update available",
                $"A new version has been released!\n{e.Release.TagName} is now available for install. Click here to install and update!",
                ToolTipIcon.Info);
            installUpdateMenuItem.Visible = true;
        }

        private void closeMenuItem_Click(object sender, EventArgs e)
        {
            MainService.Exit();
        }

        private void installUpdateMenuItem_Click(object sender, EventArgs e)
        {
            if (LatestRelease == null)
                return;

            MainService.InstallUpdate(LatestRelease);
        }

        private async void checkUpdatesMenuItem_Click(object sender, EventArgs e)
        {
            bool hasUpdate = await UpdateService.CheckForUpdates();
            if (!hasUpdate)
            {
                appTrayIcon.ShowBalloonTip(
                10000,
                "No updates",
                $"You are already running the latest version!",
                ToolTipIcon.None);
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;
            if (RunOptions.Update)
            {
                appTrayIcon.ShowBalloonTip(
                    10000,
                    "Updated successfully!",
                    $"Raid Toolkit has been updated to v{ThisAssembly.AssemblyFileVersion}!",
                    ToolTipIcon.Info);
            }
        }
    }
}
