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
        private GitHub.Updater Updater;
        public UpdateService(ILogger<UpdateService> logger, GitHub.Updater updater)
        {
            Logger = logger;
            Updater = updater;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var release = await Updater.GetLatest();
                // using (var mgr = new Squirrel.UpdateManager("C:\\Projects\\MyApp\\Releases"))
                {
                    await Task.Delay(-1, stoppingToken);
                }
            }
            catch { }
        }
    }
}