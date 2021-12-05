using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SuperSocket;

namespace Raid.Service
{
    public class MainService : BackgroundService
    {
        private readonly DataUpdateSettings DataSettings;
        private readonly RaidInstanceFactory Factory;
        private readonly ILogger<MainService> Logger;
        private readonly IHostApplicationLifetime Lifetime;
        private readonly UpdateService UpdateService;
        private readonly IServiceProvider ServiceProvider;
        private readonly SessionFactory SessionFactory;
        private readonly ErrorService ErrorService;

        private DateTime ActiveUntil = DateTime.UtcNow;

        public MainService(
            ProcessWatcherService processWatcher,
            ILogger<MainService> logger,
            RaidInstanceFactory factory,
            UpdateService updateService,
            IServiceProvider serviceProvider,
            IHostApplicationLifetime appLifetime,
            IOptions<AppSettings> settings,
            MemoryLogger memoryLogger,
            ISessionFactory sessionFactory,
            ErrorService errorService)
        {
            Factory = factory;
            Logger = logger;
            Lifetime = appLifetime;
            UpdateService = updateService;
            ServiceProvider = serviceProvider;
            DataSettings = settings.Value.DataSettings;
            SessionFactory = sessionFactory as SessionFactory;
            ErrorService = errorService;
            processWatcher.ProcessFound += OnProcessFound;
            processWatcher.ProcessClosed += OnProcessClosed;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Application.ThreadException += OnThreadException;
        }

        private void OnProcessFound(object sender, ProcessWatcherService.ProcessWatcherEventArgs e)
        {
            using TrackedOperation createInstanceOp = ErrorService.TrackOperation(ServiceErrorCategory.Process, e.Id.ToString(), e.Id);
            try
            {
                _ = Factory.Create(e.Process, ServiceProvider.CreateScope());
                ErrorService.ClearError(ServiceErrorCategory.Process, e.Id.ToString());
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                if (ex.NativeErrorCode == 5) // access denied
                {
                    createInstanceOp.Fail(ServiceError.ProcessAccessDenied);
                    Logger.LogError(ServiceError.ProcessAccessDenied.EventId(), ex, "Process cannot be accessed, is it running as administrator?");
                    e.Retry = false;
                }
                else
                {
                    createInstanceOp.Fail(ServiceError.AccountNotReady, 5);
                    Logger.LogError(ServiceError.AccountNotReady.EventId(), ex, "Account is not ready");
                    e.Retry = true;
                }
            }
            catch (Exception ex)
            {
                createInstanceOp.Fail(ServiceError.AccountNotReady, 5);
                Logger.LogError(ServiceError.AccountNotReady.EventId(), ex, "Account is not ready");
                e.Retry = true;
            }
        }

        private void OnProcessClosed(object sender, ProcessWatcherService.ProcessWatcherEventArgs e)
        {
            try
            {
                Factory.Destroy(e.Id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ServiceError.AccountNotReady.EventId(), ex, "Failed to dispose account instance");
            }
        }

        private void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Logger.LogError(ServiceError.ThreadException.EventId(), e.Exception, "Unhandled thread exception");
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.LogError(ServiceError.UnhandledException.EventId(), (Exception)e.ExceptionObject, "Unhandled exception");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Delay(-1, stoppingToken);
            }
            catch { }
        }

        public void Run(RunOptions options)
        {
            Console.CancelKeyPress += (sender, e) => Application.Exit();
            _ = TaskExtensions.RunAfter(1, UpdateAccounts);

            _ = Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.Run(ServiceProvider.GetRequiredService<UI.MainWindow>());
        }

        public void Restart(bool postUpdate = false)
        {
            List<string> args = new() { "--wait", "30000" };
            if (postUpdate)
                args.Add("--post-update");

            _ = Process.Start(AppConfiguration.ExecutablePath, args.ToArray());
            Exit();
        }

        public void Exit()
        {
            TaskExtensions.Shutdown();
            Application.Exit();
            Lifetime.StopApplication();
        }

        public async void InstallUpdate(GitHub.Schema.Release release)
        {
            try
            {
                await UpdateService.InstallRelease(release);
                Restart(postUpdate: true);
            }
            catch { }
        }

        private void OnClose(object sender, EventArgs e)
        {
            Exit();
        }

        private void UpdateAccounts()
        {
            ActiveUntil = SessionFactory.LastSessionActive.AddMilliseconds(DataSettings.ActiveCooldownMs);
            bool isActive = ActiveUntil > DateTime.UtcNow;
            int nextDelay = isActive ? DataSettings.ActiveIntervalMs : DataSettings.IdleIntervalMs;

            if (Model.ModelAssemblyResolver.CurrentVersion != Model.ModelAssemblyResolver.LoadedVersion)
            {
                Restart();
                return;
            }

            Logger.LogDebug($"Updating game data for ({Factory.Instances.Count}) processes.");

            foreach (var instance in Factory.Instances.Values)
            {
                try
                {
                    instance.Update();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ServiceError.AccountUpdateFailed.EventId(), ex, $"Failed to update account {instance.Id}");
                }
            }

            Logger.LogDebug($"Scheduling next update in {nextDelay}ms (active={isActive};until={ActiveUntil:o})");
            _ = TaskExtensions.RunAfter(nextDelay, UpdateAccounts);
        }
    }
}
