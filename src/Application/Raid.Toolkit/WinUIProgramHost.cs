using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.UI.Dispatching;

using Raid.Toolkit.Application.Core;
using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Application.Core.Host;

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

        public Task Start(IHost host, Action startupFunction)
        {
            NotificationManager notificationManager = new();
            notificationManager.Init();
            XamlCheckProcessRequirements();
            WinRT.ComWrappersSupport.InitializeComWrappers();
            //ToastNotificationManagerCompat.OnActivated += ToastNotificationManagerCompat_OnActivated;

            AppHost.Start(host);

            XamlApplication.Start(async (p) =>
            {
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
                    try
                    {
                        lifetimeService.StopApplication();
                        await host.StopAsync();
                    }
                    catch { }
                    XamlApplication.Current.Exit();
                }
            });
            notificationManager.Unregister();
            return Task.CompletedTask;
        }

        private void ToastNotificationManagerCompat_OnActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            throw new NotImplementedException();
        }
    }
}
