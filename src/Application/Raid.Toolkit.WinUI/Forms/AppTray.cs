using Microsoft.Extensions.Hosting;
using Raid.Toolkit.App;
using Raid.Toolkit.App.Tasks.Base;
using Raid.Toolkit.WinUI;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raid.Toolkit.Forms
{
    public class AppTray : IDisposable, IHostedService
    {
        private ContextMenuStrip? ContextMenu;
        private NotifyIcon? NotifyIcon;
        private readonly IAppUI AppUI;
        private readonly AppService AppService;
        private bool IsDisposed;

        public AppTray(AppService appService, IAppUI appUI)
        {
            AppService = appService;
            AppUI = appUI;
        }

        private void NotifyIcon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            AppUI.ShowMainWindow();
        }

        private void Exit()
        {
            AppService.Exit();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    RTKApplication.Current.UIContext.Post(signal =>
                    {
                        if (NotifyIcon != null)
                        {
                            NotifyIcon?.Dispose();
                            NotifyIcon = null;
                        }
                        if (ContextMenu!= null)
                        {
                            ContextMenu?.Dispose();
                            ContextMenu = null;
                        }
                    }, null);
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
                ContextMenu = new();
                ContextMenu.Items.Add(new ToolStripMenuItem("Exit", null, (_, _) => Exit()));

                NotifyIcon = new()
                {
                    Text = "Raid Toolkit",
                    Icon = FormsResources.AppIcon,
                    Visible = true,
                    ContextMenuStrip = ContextMenu
                };
                NotifyIcon.MouseClick += NotifyIcon_MouseClick;
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
