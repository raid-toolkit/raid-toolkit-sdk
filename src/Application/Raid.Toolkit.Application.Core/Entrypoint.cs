using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.Application.Core.Commands;
using Raid.Toolkit.Application.Core.Commands.Base;

namespace Raid.Toolkit.Application.Core
{
    public class Entrypoint<TAppUI, TProgramHost>
        where TAppUI : class, IAppUI, IHostedService, IDisposable
        where TProgramHost : class, IProgramHost
    {
        private readonly IServiceProvider ServiceProvider;

        public Entrypoint()
        {
            IServiceCollection services = new ServiceCollection()
                .AddTypesAssignableTo<ICommandTaskMatcher>(services => services.AddSingleton)
                .AddSingleton<CommandTaskManager>()
                .AddSingleton<IProgramHost, TProgramHost>()
                .AddSingleton<IAppHostBuilder, AppHostBuilder<TAppUI>>();

            ServiceProvider = services.BuildServiceProvider();
        }

        public T CreateInstance<T>(params object[] parameters)
        {
            return ActivatorUtilities.CreateInstance<T>(ServiceProvider, parameters);
        }
    }
}
