using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Raid.Toolkit.Extensibility.Host
{
    public class ApplicationHost : BackgroundService
    {
        private readonly IExtensionHostController ExtensionHost;

        public ApplicationHost(IExtensionHostController extensionHost)
        {
            ExtensionHost = extensionHost;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await ExtensionHost.LoadExtensions();
            cancellationToken.ThrowIfCancellationRequested();

            ExtensionHost.ActivateExtensions();
            cancellationToken.ThrowIfCancellationRequested();
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            // TODO: call deactivate on all extensions
            return base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}