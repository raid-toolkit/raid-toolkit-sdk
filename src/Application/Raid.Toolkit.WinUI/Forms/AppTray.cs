using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Raid.Toolkit.App.Tasks.Base;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Model;
using Raid.Toolkit.WinUI;

namespace Raid.Toolkit.Forms
{
    public partial class AppTray : IDisposable, IHostedService
    {
        private readonly IAppUI AppUI;
        private readonly IServiceProvider ServiceProvider;
        private readonly IOptions<ProcessManagerSettings> Settings;
        private readonly PlariumPlayAdapter PPAdapter = new();

        private AppTrayMenu? appTrayMenu;
        private NotifyIcon? NotifyIcon;
        private Action? OnClickCallback;

        private bool IsDisposed;

        public AppTray(
            IServiceProvider serviceProvider,
            IAppUI appUI,
            IOptions<ProcessManagerSettings> settings)
        {
            ServiceProvider = serviceProvider;
            AppUI = appUI;
            Settings = settings;
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private void OnAppTrayIconClicked()
        {
            if (!RegistrySettings.ClickToStart)
            {
                AppUI.ShowMain();
                return;
            }

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

        internal void ShowNotification(string title, string description, ToolTipIcon icon, int timeoutMs, Action? onActivate)
        {
            OnClickCallback = onActivate;
            var notifyIcon = NotifyIcon;
            if (notifyIcon == null)
                return;

            RTKApplication.Post(() =>
            {
                notifyIcon.ShowBalloonTip(timeoutMs, title, description, icon);
            });
        }

        private void OnBaloonTipClosed(object? sender, EventArgs e)
        {
            OnClickCallback = null;
        }

        private void OnBaloonTipClicked(object? sender, EventArgs e)
        {
            OnClickCallback?.Invoke();
            OnClickCallback = null;
        }

        private void NotifyIcon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            OnAppTrayIconClicked();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    RTKApplication.Post(() =>
                    {
                        if (NotifyIcon != null)
                        {
                            NotifyIcon?.Dispose();
                            NotifyIcon = null;
                        }
                        if (appTrayMenu != null)
                        {
                            appTrayMenu?.Dispose();
                            appTrayMenu = null;
                        }
                    });
                }

                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            TaskCompletionSource startedSignal = new();
            RTKApplication.Current.UIContext.Post(signal =>
            {
                if (signal is not TaskCompletionSource tcs) throw new InvalidOperationException();

                appTrayMenu = ActivatorUtilities.CreateInstance<AppTrayMenu>(ServiceProvider);
                NotifyIcon = new()
                {
                    Text = "Raid Toolkit",
                    Icon = FormsResources.AppIcon,
                    Visible = true,
                    ContextMenuStrip = appTrayMenu
                };
                NotifyIcon.MouseClick += NotifyIcon_MouseClick;
                NotifyIcon.BalloonTipClosed += OnBaloonTipClosed;
                NotifyIcon.BalloonTipClicked += OnBaloonTipClicked;
                tcs.SetResult();
            }, startedSignal);
            return startedSignal.Task;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
