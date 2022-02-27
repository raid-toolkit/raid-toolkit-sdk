using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host
{
    public abstract class IdleBackgroundService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Delay(-1, stoppingToken);
            }
            catch { }
        }
    }
}
