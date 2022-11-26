using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Windows.AppNotifications;

using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Extensibility;

namespace Raid.Toolkit.UI.WinUI
{
    public class AppWinUI : IAppUI, IHostedService, IDisposable
    {
        private MainWindow? MainWindow;
        private readonly IServiceProvider ServiceProvider;
        private MainWindow MainWindowUnsafe => MainWindow ??= ActivatorUtilities.CreateInstance<MainWindow>(ServiceProvider);

        private bool IsDisposed;
        private AppTray? AppTray;

        public SynchronizationContext? SynchronizationContext { get; }

        public AppWinUI(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            SynchronizationContext = SynchronizationContext.Current;
        }

        #region Dispatch
        public void Dispatch(Action action)
        {
            if (SynchronizationContext.Current == SynchronizationContext)
                action();
            else
                SynchronizationContext?.Post(_ => action(), null);
        }

        public void Dispatch<TState>(Action<TState> action, TState state)
        {
            if (SynchronizationContext.Current == SynchronizationContext)
                action(state);
            else
                SynchronizationContext?.Post(state => action((TState)state!), state);
        }

        public Task Post(Action action)
        {
            TaskCompletionSource signal = new();
            Dispatch(() =>
            {
                try
                {
                    action();
                    signal.SetResult();
                }
                catch (Exception ex)
                {
                    signal.SetException(ex);
                }
            });
            return signal.Task;
        }

        public Task<T> Post<T>(Func<T> action)
        {
            TaskCompletionSource<T> signal = new();
            Dispatch(() =>
            {
                try
                {
                    T result = action();
                    signal.SetResult(result);
                }
                catch (Exception ex)
                {
                    signal.SetException(ex);
                }
            });
            return signal.Task;
        }

        public Task<T> Post<T, U>(Func<U, T> action, U state)
        {
            TaskCompletionSource<T> signal = new();
            Dispatch(() =>
            {
                try
                {
                    T result = action(state);
                    signal.SetResult(result);
                }
                catch (Exception ex)
                {
                    signal.SetException(ex);
                }
            });
            return signal.Task;
        }
        #endregion Dispatch

        public void ShowMain()
        {
            Post(() =>
            {
                MainWindowUnsafe.Activate();
                MainWindowUnsafe.BringToFront();
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

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                Post(() =>
                {
                    if (disposing)
                    {
                        MainWindow?.Close();
                        System.Windows.Forms.Application.Exit();
                    }

                    MainWindow = null;
                });

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
            Post(() =>
            {
                MainWindowUnsafe.OpenSettings();
                ShowMain();
            });
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Post(() =>
            {
                AppTray = ActivatorUtilities.CreateInstance<AppTray>(ServiceProvider);
            });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void ShowErrors()
        {
            throw new NotImplementedException();
        }

        public void ShowExtensionManager()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            MainWindow ??= ActivatorUtilities.CreateInstance<MainWindow>(ServiceProvider);
        }
    }
}
