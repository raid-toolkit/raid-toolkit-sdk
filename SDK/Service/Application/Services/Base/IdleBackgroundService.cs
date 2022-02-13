using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Raid.Service
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
