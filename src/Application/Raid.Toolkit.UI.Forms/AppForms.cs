using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Raid.Toolkit.Application.Core.Tasks.Base;
using Raid.Toolkit.Extensibility;

using FormsApplication = System.Windows.Forms.Application;

namespace Raid.Toolkit.UI.Forms
{
    public class AppForms : IAppUI, IHostedService, IDisposable
    {
        //private SplashScreen? SplashScreen;
        private readonly IServiceProvider ServiceProvider;
        private bool IsDisposed;
        private SynchronizationContext? UIContext;

        private AppTray? AppTray;
        private MainWindow? MainWindow;

        public AppForms(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            FormsApplication.SetHighDpiMode(HighDpiMode.SystemAware);
            FormsApplication.EnableVisualStyles();
            FormsApplication.SetCompatibleTextRenderingDefault(false);
        }

        public void SetSynchronizationContext(SynchronizationContext context)
        {
            UIContext = context;
        }

        #region Dispatch
        public void Dispatch(Action action)
        {
            if (SynchronizationContext.Current == UIContext)
                action();
            else
                UIContext?.Post(_ => action(), null);
        }

        public void Dispatch<TState>(Action<TState> action, TState state)
        {
            if (SynchronizationContext.Current == UIContext)
                action(state);
            else
                UIContext?.Post(state => action((TState)state!), state);
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
                MainWindow = ActivatorUtilities.CreateInstance<MainWindow>(ServiceProvider);
                MainWindow.Show();
            });
        }

        public void ShowInstallUI()
        {
            //throw new NotImplementedException();
        }

        public bool? ShowExtensionInstaller(ExtensionBundle bundleToInstall)
        {
            throw new NotImplementedException();
        }

        public void ShowNotification(string title, string description, System.Windows.Forms.ToolTipIcon icon, int timeoutMs, Action? onActivate = null)
        {
            throw new NotImplementedException();
        }

        public void ShowSettings()
        {
            throw new NotImplementedException();
        }

        public void ShowErrors()
        {
            throw new NotImplementedException();
        }

        public void ShowExtensionManager()
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Post(() =>
            {
                AppTray = new(ServiceProvider);
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    AppTray?.Dispose();
                }

                AppTray = null;

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
