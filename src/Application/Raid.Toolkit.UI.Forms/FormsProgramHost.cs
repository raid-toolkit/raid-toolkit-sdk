using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Raid.Toolkit.Application.Core;
using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.Application.Core.Commands.Base;

using FormsApplication = System.Windows.Forms.Application;
using Raid.Toolkit.Extensibility.Interfaces;

namespace Raid.Toolkit.UI.Forms
{
    public class FormsProgramHost : IProgramHost
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public async Task Start(IHost host, Action startupFunction)
        {
            AppHost.Start(host);

            using (IAppUI? appUI = host.Services.GetService<IAppUI>())
            {
                IAppService? appService = host.Services.GetService<IAppService>();
                _ = appService?.WaitForStop().ContinueWith(_ => FormsApplication.Exit());

                startupFunction();

                appUI?.Run();
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
