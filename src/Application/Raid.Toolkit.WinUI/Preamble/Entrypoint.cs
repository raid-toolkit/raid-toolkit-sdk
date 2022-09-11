using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Raid.Toolkit.App.Tasks;
using Raid.Toolkit.App.Tasks.Base;

namespace Raid.Toolkit.Preamble
{
    internal class Entrypoint
    {
        private IServiceProvider ServiceProvider;

        public Entrypoint()
        {
            IServiceCollection services = new ServiceCollection()
                .AddTypesAssignableTo<ICommandTaskMatcher>(services => services.AddSingleton)
                .AddSingleton<CommandTaskManager>();

            ServiceProvider = services.BuildServiceProvider();
        }

        public T CreateInstance<T>(params object[] parameters)
        {
            return ActivatorUtilities.CreateInstance<T>(ServiceProvider, parameters);
        }
    }
}
