using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Raid.Toolkit.Extensibility.Host.Services
{
    public class ProcessWatcherService : PollingBackgroundService
    {
        private protected override TimeSpan PollInterval { get; }

        private readonly IProcessManager ProcessManager;
        private readonly IOptions<ProcessManagerSettings> Settings;
        private readonly ErrorService ErrorService;
        private readonly IGameInstanceManager GameInstanceManager;

        public ProcessWatcherService(
            ILogger<ProcessWatcherService> logger,
            IProcessManager processManager,
            IOptions<ProcessManagerSettings> settings,
            ErrorService errorService,
            IGameInstanceManager gameInstanceManager
            ) : base(logger)
        {
            ProcessManager = processManager;
            Settings = settings;
            ErrorService = errorService;
            GameInstanceManager = gameInstanceManager;
            PollInterval = new(0, 0, 0, 0, Settings.Value.PollIntervalMs);

            ProcessManager.ProcessFound += OnProcessFound;
            ProcessManager.ProcessClosed += OnProcessClosed;
        }

        private void OnProcessFound(object sender, IProcessManager.ProcessEventArgs e)
        {
            using TrackedOperation createInstanceOp = ErrorService.TrackOperation(
                ServiceErrorCategory.Process,
                e.Id.ToString(),
                e.Id);
            try
            {
                GameInstanceManager.AddInstance(e.Process);
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

        private void OnProcessClosed(object sender, IProcessManager.ProcessEventArgs e)
        {
            try
            {
                GameInstanceManager.RemoveInstance(e.Id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ServiceError.AccountNotReady.EventId(), ex, "Failed to dispose account instance");
            }
        }

        protected override Task ExecuteOnceAsync(CancellationToken token)
        {
            ProcessManager.Refresh();
            return Task.CompletedTask;
        }
    }
}
