using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Raid.Service
{
    public class UpdateService : BackgroundService
    {
        private ILogger<UpdateService> Logger;
        public UpdateService(ILogger<UpdateService> logger)
        {
            Logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using (var mgr = new Squirrel.UpdateManager("C:\\Projects\\MyApp\\Releases"))
                {
                    await Task.Delay(-1, stoppingToken);
                }
            }
            catch { }
        }
    }
}