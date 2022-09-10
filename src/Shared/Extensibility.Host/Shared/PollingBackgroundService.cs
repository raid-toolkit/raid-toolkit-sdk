using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Raid.Toolkit.Extensibility.Host
{
    public abstract class PollingBackgroundService : IHostedService, IDisposable
    {
        protected readonly ILogger Logger;
        private static readonly TimeSpan DefaultPollInterval = new(0, 1, 0);
        private protected virtual TimeSpan PollInterval => DefaultPollInterval;

        private Task CurrentTask;
        private CancellationTokenSource CurrentTaskCanellationTokenSource;
        private bool IsDisposed;

        public PollingBackgroundService(ILogger logger)
        {
            Logger = logger;
        }

        protected abstract Task ExecuteOnceAsync(CancellationToken token);

        public Task StartAsync(CancellationToken cancellationToken)
        {
            CurrentTaskCanellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            CurrentTask = Task.Run(() => ExecuteAsync(cancellationToken), cancellationToken);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (CurrentTask == null)
                return;

            try
            {
                CurrentTaskCanellationTokenSource.Cancel();
            }
            finally
            {
                await Task.WhenAny(CurrentTask, Task.Delay(Timeout.Infinite, cancellationToken)).ConfigureAwait(false);
            }
        }

        private async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ExecuteOnceAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Exception thrown from polling service");
                }

                // ensure delay is included if an exception is thrown above
                try
                {
                    await Task.Delay((int)PollInterval.TotalMilliseconds, stoppingToken);
                }
                catch (OperationCanceledException) // expected if the service is shutting down
                { }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    CurrentTaskCanellationTokenSource?.Cancel();
                }

                CurrentTaskCanellationTokenSource = null;
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
