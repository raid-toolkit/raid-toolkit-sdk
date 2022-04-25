using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Extensibility.Providers;

namespace Raid.Toolkit.Extensibility
{
    public class ContextDataManager : IContextDataManager
    {
        private readonly List<IDataProvider> ProvidersList = new();
        private readonly IServiceProvider ServiceProvider;

        public ContextDataManager(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IEnumerable<IDataProvider<TContext>> OfType<TContext>() where TContext : class, IDataContext
        {
            return ProvidersList
                .Where(prov => prov.ContextType == typeof(TContext))
                .Cast<IDataProvider<TContext>>();
        }

        public IDisposable AddProvider<T>(T provider) where T : IDataProvider
        {
            ProvidersList.Add(provider);
            return new HostResourceHandle(() => ProvidersList.Remove(provider));
        }

        [Obsolete]
        public IDisposable AddProvider<T>() where T : IDataProvider
        {
            IServiceProvider scope = ServiceProvider.CreateScope().ServiceProvider;
            T instance = ActivatorUtilities.CreateInstance<T>(scope);
            return AddProvider(instance);
        }
    }
}
