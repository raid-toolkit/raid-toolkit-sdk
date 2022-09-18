using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raid.Toolkit.App.Tasks.Base;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.WinUI;

namespace Raid.Toolkit.DependencyInjection
{
    public class AppWinUI : IAppUI, IDisposable
    {
        private SplashScreen? SplashScreen;
        private readonly IHostApplicationLifetime ApplicationLifetime;
        private readonly IServiceProvider ServiceProvider;
        private bool IsDisposed;

        public AppWinUI(IHostApplicationLifetime applicationLifetime, IServiceProvider serviceProvider)
        {
            ApplicationLifetime = applicationLifetime;
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

        public void ShowUpdateNotification()
        {
            //throw new NotImplementedException();
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
    }
}
