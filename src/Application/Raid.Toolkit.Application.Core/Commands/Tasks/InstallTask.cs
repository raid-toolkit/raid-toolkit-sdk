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
        private readonly InstallOptions Options;

        public InstallExtensionTask(IProgramHost program, IAppHostBuilder appHostBuilder, InstallOptions options)
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

            IHost host = AppHostBuilder.Build();
            // NB: Do not call host.StartAsync, we don't need/want any services
            
            AppHost.Start(host);

            ProgramHost.Start(host, () =>
            {
                IAppUI appUI = host.Services.GetRequiredService<IAppUI>();
                ExtensionBundle bundle = ExtensionBundle.FromDirectory(Options.PackagePath);
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