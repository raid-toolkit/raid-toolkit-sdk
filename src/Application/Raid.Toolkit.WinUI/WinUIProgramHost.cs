using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Dispatching;

using Raid.Toolkit.Application.Core;
using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.WinUI;

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using XamlApplication = Microsoft.UI.Xaml.Application;

namespace Raid.Toolkit.UI.Forms
{
    public class WinUIProgramHost : IProgramHost
    {
        static WinUIProgramHost()
        {
            WinRT.ComWrappersSupport.InitializeComWrappers();
        }

        public async Task Start(IHost host, Action startupFunction)
        {
            AppHost.Start(host);

            using (IAppUI? appUI = host.Services.GetService<IAppUI>())
            {
                XamlApplication.Start(async (p) =>
                {
                    try
                    {
                        DispatcherQueueSynchronizationContext context = new(DispatcherQueue.GetForCurrentThread());
                        SynchronizationContext.SetSynchronizationContext(context);
                        RTKApplication? app = new(context, host);
                        startupFunction();
                        appUI?.Run();
                        await app.WaitForExit();
                    }
                    finally
                    {
                        IHostApplicationLifetime lifetimeService = host.Services.GetRequiredService<IHostApplicationLifetime>();
                        try
                        {
                            lifetimeService.StopApplication();
                        }
                        catch { }
                        await host.StopAsync();
                        XamlApplication.Current.Exit();
                    }
                });
            }

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
