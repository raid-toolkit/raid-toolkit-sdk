using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Extensibility.Host;

namespace Raid.Toolkit.Application.Core.Commands.Tasks
{
    internal class SetupTask : ICommandTask
    {
        private readonly IProgramHost ProgramHost;
        private readonly IAppHostBuilder AppHostBuilder;

        public SetupTask(IProgramHost program, IAppHostBuilder appHostBuilder)
        {
            ProgramHost = program;
            AppHostBuilder = appHostBuilder;
        }

        public Task<int> Invoke()
        {
            ApplicationHost.Enabled = false;

            AppHostBuilder
                .AddLogging()
                .AddUI();

            // NB: Do not call host.StartAsync, we don't need/want any services
            IHost host = AppHostBuilder.Build();
            
            AppHost.Start(host);

            ProgramHost.Start(host, () =>
            {
                IAppUI appUI = host.Services.GetRequiredService<IAppUI>();
                appUI.ShowInstallUI();
            });

            return Task.FromResult(0);
        }
    }
}
