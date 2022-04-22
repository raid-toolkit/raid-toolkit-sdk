using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Host.Services;
using Raid.Toolkit.Model;

namespace Raid.Toolkit.UI
{
    internal partial class MainWindow : Form
    {
        private const int kDefaultBalloonTipTimeout = 10000;
        private GitHub.Schema.Release? LatestRelease;
        private readonly UpdateService UpdateService;
        private readonly AppService AppService;
        private readonly ILogger<MainWindow> Logger;
        private readonly RunOptions RunOptions;
        private readonly IOptions<ProcessManagerSettings> Settings;
        private readonly ErrorService ErrorService;
        private readonly IServiceProvider ServiceProvider;
        private readonly CachedDataStorage<PersistedDataStorage> SettingsStorage;
        private readonly IMenuManager MenuManager;
        private Action? OnClickCallback;
        private readonly PlariumPlayAdapter PPAdapter = new();
        private readonly ErrorsWindow ErrorsWindow;

        public MainWindow(
            IOptions<ProcessManagerSettings> settings,
            ILogger<MainWindow> logger,
            UpdateService updateService,
            AppService mainService,
            RunOptions runOptions,
            ErrorService errorService,
            IMenuManager menuManager,
            CachedDataStorage<PersistedDataStorage> settingsStorage,
            IServiceProvider serviceProvider)
        {
            InitializeComponent();
            RunOptions = runOptions;
            Logger = logger;
            UpdateService = updateService;
            AppService = mainService;
            Settings = settings;
            ErrorService = errorService;
            ServiceProvider = serviceProvider;
            SettingsStorage = settingsStorage;
            MenuManager = menuManager;
            ErrorsWindow = ActivatorUtilities.CreateInstance<ErrorsWindow>(ServiceProvider);

            // must trigger load here
            UpdateService.UpdateAvailable += OnUpdateAvailable;
            appTrayIcon.Text = $"Raid Toolkit v{ThisAssembly.AssemblyFileVersion}";
            appTrayIcon.Icon = Properties.Resources.AppIcon;
            appTrayIcon.Visible = true;

            // subscribe to error events
            ErrorService.OnErrorAdded += OnErrorAdded;
            foreach (var e in ErrorService.CurrentErrors.Values)
            {
                OnErrorAdded(ErrorService, e);
            }
        }

        private void OnErrorAdded(object? sender, ErrorEventArgs e)
        {
            Action onClickCallback = ShowErrors;
            string messageTitle = "Error";
            string messageText = string.Empty;
            if (e.Category == ServiceErrorCategory.Account)
            {
                e.ErrorMessage = $"Could not update account '{e.TargetDescription}'";
                e.HelpMessage = "Check the logs directory for any errors.";
            }
            else if (e.Category == ServiceErrorCategory.Process)
            {
                if (e.ErrorCode == ServiceError.ProcessAccessDenied)
                {
                    messageTitle = "Administrator access required";
                    messageText = $"Restart Raid Toolkit as Administrator?\n(not recommended)";
                    e.ErrorMessage = $"Cannot access process ({e.TargetDescription}). Is it running as administrator?";
                    e.HelpMessage = "Check to make sure the game is not running as administrator. If it is, Raid Toolkit must also be started as administrator in order to access game data";
                    onClickCallback = () =>
                    {
                        AppService.Restart(postUpdate: false, asAdmin: true, owner: this);
                    };
                }
                else
                {
                    e.ErrorMessage = $"Could not read from game process ({e.TargetDescription})";
                    e.HelpMessage = "Check your log for any more specific errors.";
                }
            }

            if (string.IsNullOrEmpty(e.ErrorMessage))
                return;

            ShowBalloonTip(kDefaultBalloonTipTimeout, messageTitle, string.IsNullOrEmpty(messageText) ? e.ErrorMessage : messageText, ToolTipIcon.Error, onClickCallback);
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private void OnAppTrayIconClicked()
        {
            if (!RegistrySettings.ClickToStart)
                return;

            var raidProcess = Process.GetProcessesByName(Settings.Value.ProcessName).FirstOrDefault();
            if (raidProcess != null)
            {
                _ = SetForegroundWindow(raidProcess.MainWindowHandle);
            }
            else
            {
                if (PPAdapter.TryGetGameVersion(101, "raid", out PlariumPlayAdapter.GameInfo gameInfo))
                {
                    _ = Process.Start(gameInfo.PlariumPlayPath, new string[] { "--args", $"-gameid=101", "-tray-start" });
                }
            }
        }

        private void InvokeIfNeeded(Action action)
        {
            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

        private void ShowErrors()
        {
            InvokeIfNeeded(() =>
            {
                ErrorsWindow.ShowDialog();
            });
        }

        private void ShowBalloonTip(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon, Action? onClickCallback)
        {
            OnClickCallback = onClickCallback;
            Action showTip = () => appTrayIcon.ShowBalloonTip(timeout, tipTitle, tipText, tipIcon);
            if (InvokeRequired)
            {
                Invoke(showTip);
            }
            else
            {
                showTip();
            }
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

            if (!SettingsStorage.TryRead<UserSettings>(SettingsDataContext.Default, ".usersettings", out UserSettings settings))
                settings = new();

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
            SettingsStorage.Write<UserSettings>(SettingsDataContext.Default, ".usersettings", settings);
            Logger.LogInformation(ServiceEvent.UserPermissionAccept.EventId(), $"Permission accepted for {origin}");
            return true;
        }

        private void OnUpdateAvailable(object? sender, UpdateService.UpdateAvailbleEventArgs e)
        {
            InvokeIfNeeded(() =>
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
            });
        }

        private void InstallUpdate()
        {
            if (LatestRelease == null)
                return;

            AppService.InstallUpdate(LatestRelease);
        }

        private void closeMenuItem_Click(object sender, EventArgs e)
        {
            AppService.Exit();
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
            bool hasUpdate = await UpdateService.CheckForUpdates(force: true);
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

        private void appTrayMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            extensionsToolStripMenuItem.DropDownItems.Clear();
            extensionsToolStripMenuItem.DropDownItems.AddRange(MenuManager.GetEntries().Select(entry =>
                new ToolStripMenuItem(entry.DisplayName, entry.Image, (sender, e) => entry.OnActivate())
            ).ToArray());
            extensionsToolStripMenuItem.Enabled = extensionsToolStripMenuItem.DropDownItems.Count > 0;
        }
    }
}
