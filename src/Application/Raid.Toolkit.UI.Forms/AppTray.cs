using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.UI.Forms
{
    internal class AppTray : IDisposable
    {
        private readonly IServiceProvider ServiceProvider;
        private AppTrayMenu? AppTrayMenu;
        private NotifyIcon? NotifyIcon;
        private Action? OnClickCallback;
        private bool IsDisposed;

        public AppTray(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;

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
        }

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

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~AppTray()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
