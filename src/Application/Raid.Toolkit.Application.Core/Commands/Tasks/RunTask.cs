using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Raid.Toolkit.Application.Core.DependencyInjection;
using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Application.Core.Commands.Matchers;
using Raid.Toolkit.Common;

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
            if (!Options.Standalone)
            {
                await SingletonProcess.TryAcquireSingletonWithTimeout(Options.Wait ?? 0);
            }

            AppHostBuilder
                .AddExtensibility()
                .AddLogging()
                .AddUI()
                .AddAppServices()
                .AddWebSockets(AppHost.HandleMessage);

            IHost host = AppHostBuilder.Build();
            ConfigureHost(host);

            AppHost.Start(host);

            // must allow AppUI to initialize any process hooks before
            // the synchronization context is requested

            await ProgramHost.Start(host, () =>
            {
                _ = Task.Run(() => host.StartAsync());
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
