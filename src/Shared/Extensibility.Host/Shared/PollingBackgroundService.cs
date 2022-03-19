using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Raid.Toolkit.Extensibility.Host
{
    public abstract class PollingBackgroundService : BackgroundService
    {
        protected readonly ILogger Logger;
        private static readonly TimeSpan DefaultPollInterval = new(0, 1, 0);
        private protected virtual TimeSpan PollInterval => DefaultPollInterval;

        public PollingBackgroundService(ILogger logger)
        {
            Logger = logger;
        }

        protected abstract Task ExecuteOnceAsync(CancellationToken token);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
    }
}
