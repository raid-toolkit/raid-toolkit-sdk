using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raid.Common;
using Raid.Model;

namespace Raid.Service.UI
{
    public partial class MainWindow : Form
    {
        private const int kDefaultBalloonTipTimeout = 10000;
        private GitHub.Schema.Release LatestRelease;
        private readonly UpdateService UpdateService;
        private readonly MainService MainService;
        private readonly UserData UserData;
        private readonly ILogger<MainWindow> Logger;
        private readonly RunOptions RunOptions;
        private readonly ProcessWatcherSettings Settings;
        private readonly ErrorService ErrorService;
        private readonly IServiceProvider ServiceProvider;
        private Action OnClickCallback;

        public MainWindow(
            IOptions<AppSettings> settings,
            ILogger<MainWindow> logger,
            UpdateService updateService,
            MainService mainService,
            UserData userData,
            RunOptions runOptions,
            ErrorService errorService,
            IServiceProvider serviceProvider)
        {
            InitializeComponent();
            RunOptions = runOptions;
            Logger = logger;
            UpdateService = updateService;
            UserData = userData;
            MainService = mainService;
            Settings = settings.Value.ProcessWatcher;
            ErrorService = errorService;
            ServiceProvider = serviceProvider;

            // must trigger load here
            UserData.Load();
            UpdateService.UpdateAvailable += OnUpdateAvailable;
            appTrayIcon.Text = $"Raid Toolkit v{ThisAssembly.AssemblyFileVersion}";
            appTrayIcon.Icon = Icon.ExtractAssociatedIcon(AppConfiguration.ExecutablePath);
            appTrayIcon.Visible = true;

            // subscribe to error events
            ErrorService.OnErrorAdded += OnErrorAdded;
        }

        private void OnErrorAdded(object sender, ErrorEventArgs e)
        {
            if (e.Category == ServiceErrorCategory.Account)
            {
                e.ErrorMessage = $"Could not update account '{e.TargetDescription}'";
                e.HelpMessage = "Check the logs directory for any errors.";
            }
            else if (e.Category == ServiceErrorCategory.Process)
            {
                if (e.ErrorCode == ServiceError.ProcessAccessDenied)
                {
                    e.ErrorMessage = $"Cannot access process ({e.TargetDescription}). Is it running as administrator?";
                    e.HelpMessage = "Check to make sure the game is not running as administrator. If it is, Raid Toolkit must also be started as administrator in order to access game data";
                }
                else
                {
                    e.ErrorMessage = $"Could not read from game process ({e.TargetDescription})";
                    e.HelpMessage = "Check your log for any more specific errors.";
                }
            }

            if (string.IsNullOrEmpty(e.ErrorMessage))
                return;

            ShowBalloonTip(kDefaultBalloonTipTimeout, "Error", e.ErrorMessage, ToolTipIcon.Error, ShowErrors);
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

        private void ShowErrors()
        {
            _ = ServiceProvider.GetRequiredService<ErrorsWindow>().ShowDialog();
        }

        private void ShowBalloonTip(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon, Action onClickCallback)
        {
            OnClickCallback = onClickCallback;
            appTrayIcon.ShowBalloonTip(timeout, tipTitle, tipText, tipIcon);
        }

        public bool RequestPermissions(string origin)
        {
            if (string.IsNullOrEmpty(origin))
                return true;

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
            ShowBalloonTip(
                kDefaultBalloonTipTimeout,
                "Update available",
                $"A new version has been released!\n{e.Release.TagName} is now available for install. Click here to install and update!",
                ToolTipIcon.Info,
                InstallUpdate);
            installUpdateMenuItem.Visible = true;
        }

        private void InstallUpdate()
        {
            if (LatestRelease == null)
                return;

            MainService.InstallUpdate(LatestRelease);
        }

        private void closeMenuItem_Click(object sender, EventArgs e)
        {
            MainService.Exit();
        }

        private void appTrayIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            OnClickCallback?.Invoke();
        }

        private void appTrayIcon_BalloonTipClosed(object sender, EventArgs e)
        {
            OnClickCallback = null;
        }

        private void installUpdateMenuItem_Click(object sender, EventArgs e)
        {
            InstallUpdate();
        }

        private async void checkUpdatesMenuItem_Click(object sender, EventArgs e)
        {
            bool hasUpdate = await UpdateService.CheckForUpdates();
            if (!hasUpdate)
            {
                ShowBalloonTip(
                    kDefaultBalloonTipTimeout,
                    "No updates",
                    $"You are already running the latest version!",
                    ToolTipIcon.None,
                    null);
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;
            if (RunOptions.Update)
            {
                ShowBalloonTip(
                    kDefaultBalloonTipTimeout,
                    "Updated successfully!",
                    $"Raid Toolkit has been updated to v{ThisAssembly.AssemblyFileVersion}!",
                    ToolTipIcon.Info,
                    null);
            }
        }

        private void settingsMenuItem_Click(object sender, EventArgs e)
        {
            using SettingsWindow settingsWindow = new();
            _ = settingsWindow.ShowDialog();
        }

        private void appTrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            OnAppTrayIconClicked();
        }

        private void viewErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowErrors();
        }
    }
}
