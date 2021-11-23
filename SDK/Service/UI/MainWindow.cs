using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raid.Common;
using Raid.Model;

namespace Raid.Service.UI
{
    public partial class MainWindow : Form
    {
        private GitHub.Schema.Release LatestRelease;
        private readonly UpdateService UpdateService;
        private readonly MainService MainService;
        private readonly UserData UserData;
        private readonly ILogger<MainWindow> Logger;
        private readonly RunOptions RunOptions;
        private readonly ProcessWatcherSettings Settings;

        public MainWindow(IOptions<AppSettings> settings, ILogger<MainWindow> logger, UpdateService updateService, MainService mainService, UserData userData, RunOptions runOptions)
        {
            InitializeComponent();
            RunOptions = runOptions;
            Logger = logger;
            UpdateService = updateService;
            UserData = userData;
            MainService = mainService;
            Settings = settings.Value.ProcessWatcher;

            // must trigger load here
            UserData.Load();
            UpdateService.UpdateAvailable += OnUpdateAvailable;
            appTrayIcon.Text = $"Raid Toolkit v{ThisAssembly.AssemblyFileVersion}";
            appTrayIcon.Icon = Icon.ExtractAssociatedIcon(AppConfiguration.ExecutablePath);
            appTrayIcon.Visible = true;
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private void OnAppTrayIconClicked()
        {
            if (!RegistrySettings.ClickToStart)
                return;

            var raidProcess = Process.GetProcessesByName(Settings.ProcessName).FirstOrDefault();
            if (raidProcess != null)
            {
                _ = SetForegroundWindow(raidProcess.MainWindowHandle);
            }
            else
            {
                var gameInfo = ModelAssemblyResolver.GameInfo;
                _ = Process.Start(gameInfo.PlariumPlayPath, new string[] { "--args", $"-gameid=101", "-tray-start" });
            }
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

            _ = settings.AllowedOrigins.Add(origin.ToLowerInvariant());
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

        private void settingsMenuItem_Click(object sender, EventArgs e)
        {
            using SettingsWindow settingsWindow = new();
            settingsWindow.ShowDialog();
        }

        private void appTrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            OnAppTrayIconClicked();
        }
    }
}
