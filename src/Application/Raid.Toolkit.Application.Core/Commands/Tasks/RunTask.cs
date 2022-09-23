using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Raid.Toolkit.Application.Core.DependencyInjection;
using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.Application.Core.Tasks.Base;
using Raid.Toolkit.Application.Core.Tasks.Matchers;

namespace Raid.Toolkit.Application.Core.Commands.Tasks
{
    internal class RunTask : ICommandTask
    {
        private readonly IProgramHost ProgramHost;
        private readonly IAppHostBuilder AppHostBuilder;
        private readonly RunOptions Options;

        public RunTask(IProgramHost programHost, IAppHostBuilder appHostBuilder, RunOptions options)
        {
            ProgramHost = programHost;
            AppHostBuilder = appHostBuilder;
            Options = options;
        }

        public async Task<int> Invoke()
        {
            AppHostBuilder.AddWebSockets(AppHost.HandleMessage)
                .AddExtensibility()
                .AddLogging()
                .AddUI()
                .AddAppServices();

            IHost host = AppHostBuilder.Build();
            ConfigureHost(host);

            AppHost.Start(host);

            await ProgramHost.Start(host, async (synchronizationContext) =>
            {
                IAppUI appUI = host.Services.GetRequiredService<IAppUI>();
                appUI.ShowMain();
                await Task.Run(() => host.StartAsync());
            });

            return 0;
        }

        private void ConfigureHost(IHost host)
        {
            IModelLoader modelLoader = host.Services.GetRequiredService<IModelLoader>();

            if (Options.DebugPackage == ".")
            {
                Options.DebugPackage = Environment.GetEnvironmentVariable("DEBUG_PACKAGE_DIR") ?? ".";
            }
            PackageManager.DebugPackage = Options.DebugPackage;
            PackageManager.NoDefaultPackages = Options.NoDefaultPackages;
            if (!string.IsNullOrEmpty(PackageManager.DebugPackage))
            {
                Options.Debug = true;
                string debugInteropDirectory = Path.Combine(PackageManager.DebugPackage, @"temp~interop");
                modelLoader.OutputDirectory = debugInteropDirectory;
            }
            if (!string.IsNullOrEmpty(Options.InteropDirectory))
            {
                modelLoader.OutputDirectory = Options.InteropDirectory;
            }
        }
    }
}
