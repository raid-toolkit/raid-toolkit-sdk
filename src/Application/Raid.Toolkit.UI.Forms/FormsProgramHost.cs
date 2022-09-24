using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Raid.Toolkit.Application.Core;
using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.Application.Core.Commands.Base;

namespace Raid.Toolkit.UI.Forms
{
    public class FormsProgramHost : IProgramHost
    {
        public async Task Start(IHost host, Action startupFunction)
        {
            AppHost.Start(host);

            using (IAppUI? appUI = host.Services.GetService<IAppUI>())
            {
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
