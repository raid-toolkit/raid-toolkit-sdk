using System;
using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.App.Tasks.Base;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Forms;
using Raid.Toolkit.WinUI;

namespace Raid.Toolkit.DependencyInjection
{
    public class AppWinUI : IAppUI, IDisposable
    {
        private SplashScreen? SplashScreen;
        private readonly IServiceProvider ServiceProvider;
        private bool IsDisposed;

        public AppWinUI(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public void ShowMain()
        {
            RTKApplication.Post(() =>
            {
                SplashScreen ??= ActivatorUtilities.CreateInstance<SplashScreen>(ServiceProvider);
                SplashScreen.Activate();
                _ = SplashScreen.BringToFront();
            });
        }

        public void ShowInstallUI()
        {
            //throw new NotImplementedException();
        }

        public bool? ShowExtensionInstaller(ExtensionBundle bundleToInstall)
        {
            //throw new NotImplementedException();
            return false;
        }

        public void ShowNotification(string title, string description, System.Windows.Forms.ToolTipIcon icon, int timeoutMs, Action? onActivate = null)
        {
            RTKApplication.Post(() =>
            {
                AppTray tray = ServiceProvider.GetRequiredService<AppTray>();
                tray.ShowNotification(title, description, icon, timeoutMs, onActivate);
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    SplashScreen?.Close();
                }

                SplashScreen = null;

                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void ShowSettings()
        {
            RTKApplication.Post(() =>
            {
                SettingsWindow settingsWindow = ActivatorUtilities.CreateInstance<SettingsWindow>(ServiceProvider);
                settingsWindow.Activate();
                _ = settingsWindow.BringToFront();
            });
        }

        public void ShowErrors()
        {
            throw new NotImplementedException();
        }

        public void ShowExtensionManager()
        {
            throw new NotImplementedException();
        }
    }
}
