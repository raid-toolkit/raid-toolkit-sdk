using Microsoft.Extensions.Hosting;
using Raid.Toolkit.App.Tasks.Base;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Model;
using Raid.Toolkit.WinUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raid.Toolkit.DependencyInjection
{
    public class AppWinUI : IAppUI, IDisposable
    {
        private MainWindow? MainWindow;
        private RebuildWindow? RebuildWindow;
        private readonly IHostApplicationLifetime ApplicationLifetime;
        private bool IsDisposed;

        public AppWinUI(IHostApplicationLifetime applicationLifetime)
        {
            ApplicationLifetime = applicationLifetime;
        }

        public void ShowMainWindow()
        {
            RTKApplication.Current.UIContext?.Post(_ =>
            {
                MainWindow = new();
                MainWindow.Activate();
            }, null);
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

        public void ShowRebuildUI(PlariumPlayAdapter.GameInfo gameInfo)
        {
            RTKApplication.Current.UIContext?.Post(_ =>
            {
                RebuildWindow = new(gameInfo);
                RebuildWindow.Activate();
            }, null);
        }

        public void HideRebuildUI()
        {
            RTKApplication.Current.UIContext?.Post(_ =>
            {
                RebuildWindow?.Close();
                RebuildWindow = null;
            }, null);
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
                    MainWindow?.Close();
                    RebuildWindow?.Close();
                }

                MainWindow = null;
                RebuildWindow = null;

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
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
