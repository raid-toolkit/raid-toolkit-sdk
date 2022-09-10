using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host
{
    public abstract class IdleBackgroundService : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
