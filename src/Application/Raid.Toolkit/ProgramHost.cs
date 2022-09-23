using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Raid.Toolkit.Application.Core;
using Raid.Toolkit.Application.Core.Host;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using FormsApplication = System.Windows.Forms.Application;

namespace Raid.Toolkit
{
    public class ProgramHost : IProgramHost
    {
        public async Task Start(IHost host, Func<SynchronizationContext, Task> startupFunction)
        {
            AppHost.Start(host);

            WindowsFormsSynchronizationContext synchronizationContext = new();
            SynchronizationContext.SetSynchronizationContext(synchronizationContext);

            await startupFunction(synchronizationContext);

            FormsApplication.Run();
            IHostApplicationLifetime lifetimeService = host.Services.GetRequiredService<IHostApplicationLifetime>();
            try
            {
                lifetimeService.StopApplication();
            }
            catch { }
            await host.StopAsync();
        }
    }
}
