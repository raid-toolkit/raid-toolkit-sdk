using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raid.Toolkit.Application.Core.Commands.Matchers;
using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host;

namespace Raid.Toolkit.Application.Core.Commands.Tasks
{
    internal class InstallExtensionTask : ICommandTask
    {
        private readonly IProgramHost ProgramHost;
        private readonly IAppHostBuilder AppHostBuilder;
        private readonly InstallExtensionOptions Options;

        public InstallExtensionTask(IProgramHost program, IAppHostBuilder appHostBuilder, InstallExtensionOptions options)
        {
            ProgramHost = program;
            AppHostBuilder = appHostBuilder;
            Options = options;
        }

        public Task<int> Invoke()
        {
            ApplicationHost.Enabled = false;

            AppHostBuilder
                .AddExtensibility()
                .AddLogging()
                .AddUI()
                .AddAppServices();

            // NB: Do not call host.StartAsync, we don't need/want any services
            IHost host = AppHostBuilder.Build();
            
            AppHost.Start(host);

            ProgramHost.Start(host, () =>
            {
                IAppUI appUI = host.Services.GetRequiredService<IAppUI>();
                ExtensionBundle bundle = ExtensionBundle.FromFile(Options.PackagePath);
                bool? result = appUI.ShowExtensionInstaller(bundle);
                if (result == true)
                {
                    host.Services.GetRequiredService<IPackageManager>().AddPackage(bundle);
                }
            });

            return Task.FromResult(0);
        }
    }
}
