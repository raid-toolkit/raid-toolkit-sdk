using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Google.Protobuf.WellKnownTypes;

using Microsoft.Extensions.Hosting;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

using Raid.Toolkit.App;
using Raid.Toolkit.App.Tasks.Base;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.Preamble.Commands.Matchers;
using Raid.Toolkit.WinUI;

namespace Raid.Toolkit.Preamble.Commands.Tasks
{
    internal class InstallTask : ICommandTask
    {
        private readonly InstallOptions Options;
        public InstallTask(InstallOptions options)
        {
            Options = options;
        }
        public Task<int> Invoke()
        {
            ApplicationHost.Enabled = false;

            AppHostBuilder hostBuilder = new();
            hostBuilder
                .AddExtensibility()
                .AddLogging()
                .AddUI()
                .AddAppServices();
            IHost host = hostBuilder.Build();
            // NB: Do not call host.StartAsync, we don't need/want any services
            
            AppHost.Start(host);

            Application.Start((p) =>
            {
                DispatcherQueueSynchronizationContext context = new(DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);
                _ = new RTKApplication(context, host);
                InstallExtensionWindow installWindow = new();
                installWindow.Activate();
            });

            return Task.FromResult(0);
        }
    }
}
