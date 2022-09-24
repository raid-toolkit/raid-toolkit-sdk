using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Application.Core.Commands.Matchers;
using Raid.Toolkit.Extensibility;

using FormsApplication = System.Windows.Forms.Application;

namespace Raid.Toolkit.UI.Forms
{
    public class AppForms : IAppUI, IHostedService, IDisposable
    {
        //private SplashScreen? SplashScreen;
        private readonly IServiceProvider ServiceProvider;
        private bool IsDisposed;

        private AppTray? AppTray;

        private readonly List<Form> DisposableForms = new();
        private void TrackForm(Form form)
        {
            form.Disposed += (_, _) => DisposableForms.Remove(form);
            form.FormClosed += (_, _) => form.Dispose();
        }
        private void DisposeForms()
        {
            Form[] forms = DisposableForms.ToArray();
            foreach (Form form in forms)
                form.Dispose();
        }
        private void ShowAndTrack<T>(params object[] args) where T : Form
        {
            Dispatch(() =>
            {
                T form = ActivatorUtilities.CreateInstance<T>(ServiceProvider, args);
                TrackForm(form);
                form.Show();
            });
        }

        private static readonly SynchronizationContext FormsSynchronizationContext;
        public SynchronizationContext? SynchronizationContext => FormsSynchronizationContext;

        static AppForms()
        {
            FormsApplication.SetHighDpiMode(HighDpiMode.SystemAware);
            FormsApplication.EnableVisualStyles();
            FormsApplication.SetCompatibleTextRenderingDefault(false);
            FormsSynchronizationContext = new WindowsFormsSynchronizationContext();
        }

        public AppForms(IServiceProvider serviceProvider, IOptions<RunOptions> options)
        {
            ServiceProvider = serviceProvider;
        }

        public void Run()
        {
            FormsApplication.Run();
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
            //Post(() =>
            //{
            //    MainWindow = ActivatorUtilities.CreateInstance<MainWindow>(ServiceProvider);
            //    MainWindow.Show();
            //});
        }

        public void ShowInstallUI()
        {
            ShowAndTrack<InstallWindow>();
        }

        public bool? ShowExtensionInstaller(ExtensionBundle bundleToInstall)
        {
            InstallExtensionDialog form = ActivatorUtilities.CreateInstance<InstallExtensionDialog>(ServiceProvider, bundleToInstall);
            TrackForm(form);
            DialogResult result = form.ShowDialog();
            return result == DialogResult.Yes;
        }

        public void ShowNotification(string title, string description, System.Windows.Forms.ToolTipIcon icon, int timeoutMs, Action? onActivate = null)
        {
            Dispatch(() =>
            {
                AppTray?.ShowNotification(title, description, icon, timeoutMs, onActivate);
            });
        }

        public void ShowSettings()
        {
            ShowAndTrack<SettingsWindow>();
        }

        public void ShowErrors()
        {
            ShowAndTrack<ErrorsWindow>();
        }

        public void ShowExtensionManager()
        {
            ShowAndTrack<ExtensionsWindow>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Post(() =>
            {
                AppTray = ActivatorUtilities.CreateInstance<AppTray>(ServiceProvider);
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
                    Post(() =>
                    {
                        AppTray?.Dispose();
                        DisposeForms();
                    });
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
