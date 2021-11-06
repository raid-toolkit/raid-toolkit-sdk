using System;
using System.Threading;
using System.Threading.Tasks;
using GitHub;
using GitHub.Schema;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Raid.Service
{
    public class UpdateService : BackgroundService
    {
        private readonly ILogger<UpdateService> Logger;
        private readonly Updater Updater;
        private readonly TimeSpan PollInterval;

        public UpdateService(ILogger<UpdateService> logger, IOptions<AppSettings> appSettings, Updater updater)
        {
            Logger = logger;
            Updater = updater;
            PollInterval = appSettings.Value.UpdateManager != null
                ? TimeSpan.FromMilliseconds(appSettings.Value.UpdateManager.PollIntervalMs)
                : new TimeSpan(0, 15, 0);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckForUpdates();
                try
                {
                    await Task.Delay(PollInterval.Milliseconds, stoppingToken);
                }
                catch (OperationCanceledException) // expected if the service is shutting down
                { }
            }
        }

        private async Task CheckForUpdates()
        {
            Release release = await Updater.GetLatestRelease();
            string asfv = ThisAssembly.AssemblyFileVersion;
            string asiv = ThisAssembly.AssemblyInformationalVersion;
            string releaseTag = release.TagName;
        }
    }
}