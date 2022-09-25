using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Model;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.UI.Forms
{
    internal class AppTray : IDisposable
    {
        private readonly IAppUI AppUI;
        private readonly IServiceProvider ServiceProvider;
        private readonly IOptions<ProcessManagerSettings> Settings;
        private readonly PlariumPlayAdapter PPAdapter = new();
        private AppTrayMenu? AppTrayMenu;
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

            // TODO: Move this into its own class
            AppTrayMenu = ActivatorUtilities.CreateInstance<AppTrayMenu>(ServiceProvider);
#pragma warning disable CS0436 // Type conflicts with imported type
            NotifyIcon = new()
            {
                Text = $"Raid Toolkit {ThisAssembly.AssemblyFileVersion}",
                Icon = Properties.Resources.AppIcon,
                Visible = true,
                ContextMenuStrip = AppTrayMenu
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

            notifyIcon.ShowBalloonTip(timeoutMs, title, description, icon);
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

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    AppTrayMenu?.Dispose();
                    NotifyIcon?.Dispose();
                }

                AppTrayMenu = null;
                NotifyIcon = null;
                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable
    }
}
