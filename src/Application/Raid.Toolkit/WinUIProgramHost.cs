using Client.Model.Network.GameServer;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Dispatching;

using Raid.Toolkit.Application.Core;
using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.Extensibility;

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using XamlApplication = Microsoft.UI.Xaml.Application;

namespace Raid.Toolkit.UI.WinUI
{
    public class WinUIProgramHost : IProgramHost
    {
        [DllImport("Microsoft.ui.xaml.dll")]
        private static extern void XamlCheckProcessRequirements();

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public Task Start(IHost host, Action startupFunction)
        {
            XamlCheckProcessRequirements();
            WinRT.ComWrappersSupport.InitializeComWrappers();

            AppHost.Start(host);

            TaskCompletionSource endTask = new();
            XamlApplication.Start(async (p) =>
            {
                // force loading of WinUIEx
                typeof(WinUIEx.WindowEx).FullName?.ToString();

                try
                {
                    DispatcherQueueSynchronizationContext context = new(DispatcherQueue.GetForCurrentThread());
                    SynchronizationContext.SetSynchronizationContext(context);
                    RTKApplication? app = new(context, host);

                    using IAppUI? appUI = host.Services.GetService<IAppUI>();

                    appUI?.Run();
                    startupFunction();
                    await app.WaitForExit();
                }
                finally
                {
                    IHostApplicationLifetime lifetimeService = host.Services.GetRequiredService<IHostApplicationLifetime>();
                    CancellationTokenSource tokenSource = new(5000);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Task.Run(async () =>
                    {
                        try
                        {
                            lifetimeService.StopApplication();
                            await host.StopAsync(tokenSource.Token);
                        }
                        catch (Exception e)
                        {
                            endTask.SetException(e);
                        }
                        finally
                        {
                            XamlApplication.Current.Exit();
                            endTask.TrySetResult();
                        }
                    });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            });
            return endTask.Task;
        }
    }
}
