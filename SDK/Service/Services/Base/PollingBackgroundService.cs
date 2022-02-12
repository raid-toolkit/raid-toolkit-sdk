using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Raid.Service
{
    public abstract class PollingBackgroundService : BackgroundService
    {
        private static TimeSpan DefaultPollInterval = new (0, 1, 0);
        private protected virtual TimeSpan PollInterval => DefaultPollInterval;

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
