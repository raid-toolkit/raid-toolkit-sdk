using Google.Protobuf.WellKnownTypes;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Model;
using Raid.Toolkit.UI.WinUI.Forms;

using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Raid.Toolkit.UI.WinUI
{
    public partial class AppTray : IDisposable
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

            appTrayMenu = ActivatorUtilities.CreateInstance<AppTrayMenu>(ServiceProvider);
#pragma warning disable CS0436 // Type conflicts with imported type
            NotifyIcon = new()
            {
                Text = $"Raid Toolkit {ThisAssembly.AssemblyFileVersion}",
                Icon = FormsResources.AppIcon,
                Visible = true,
                ContextMenuStrip = appTrayMenu
            };
#pragma warning restore CS0436 // Type conflicts with imported type
            NotifyIcon.MouseClick += NotifyIcon_MouseClick;
            NotifyIcon.BalloonTipClosed += OnBaloonTipClosed;
            NotifyIcon.BalloonTipClicked += OnBaloonTipClicked;
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
            
            AppUI.Dispatch(() =>
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
                    AppUI.Dispatch(() =>
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
    }
}
