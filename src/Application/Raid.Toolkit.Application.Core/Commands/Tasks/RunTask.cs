using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Application.Core.Commands.Matchers;
using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host;

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

        private static Task<int> ActivateCurrentProcess()
        {
			throw new V3NotImpl();
        }

        public async Task<int> Invoke()
        {
            if (!Options.Standalone)
            {
                if (Options.Wait.HasValue)
                {
                    await SingletonProcess.TryAcquireSingletonWithTimeout(Options.Wait.Value);
                }
                else
                {
                    // already running?
                    if (!SingletonProcess.TryAcquireSingleton())
                    {
                        return await ActivateCurrentProcess();
                    }
                }
            }

            HostFeatures hostFeatures = HostFeatures.ProcessWatcher;
            if (!Options.Debug)
                hostFeatures |= HostFeatures.AutoUpdate;

            var builder = AppHostBuilder
                .AddExtensibility()
                .AddLogging()
                .AddAppServices(hostFeatures)
                .AddUI();

            _ = AppHostBuilder.ConfigureServices(services => ProgramHost.ConfigureServices(services));

            IHost host = AppHostBuilder.Build();
            ConfigureHost();

            AppHost.Start();

            // must allow AppUI to initialize any process hooks before
            // the synchronization context is requested

            INotificationManager? notificationManager = host.Services.GetService<INotificationManager>();
            notificationManager?.Initialize();

            await ProgramHost.Start(host, () =>
            {
                _ = Task.Run(() =>
                {
                    _ = host.StartAsync();
                });
            });

            return 0;
        }

        private void ConfigureHost()
        {
            if (Options.DebugPackage == ".")
            {
                Options.DebugPackage = Environment.GetEnvironmentVariable("DEBUG_PACKAGE_DIR") ?? ".";
            }
            //PackageManager.DebugPackage = Options.DebugPackage;
            //if (!string.IsNullOrEmpty(PackageManager.DebugPackage))
            //{
            //    Options.Debug = CommonOptions.Value.Debug = true;
            //}
        }
    }
}
