using System;
using System.Linq;
using System.Collections.Generic;
using Il2CppToolkit.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Raid.DataServices;
using Raid.Toolkit.Extensibility.Providers;

namespace Raid.Toolkit.Extensibility
{
	public class ContextDataManager : IContextDataManager
	{
		private readonly List<IContextDataProvider> ProvidersList = new();
		private readonly IServiceProvider ServiceProvider;

		public ContextDataManager(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider;
		}

		public IDisposable AddProvider<T>() where T : IContextDataProvider
		{
			IServiceProvider scope = ServiceProvider.CreateScope().ServiceProvider;
			T instance = ActivatorUtilities.CreateInstance<T>(scope);
			ProvidersList.Add(instance);
			return new HostResourceHandle(() => ProvidersList.Remove(instance));
		}

		public bool Update<T>(Il2CsRuntimeContext runtime, T context) where T : IDataContext
		{
			bool updated = false;
			foreach(var provider in ProvidersList.Where(prov => prov.ContextType == typeof(T)))
			{
				updated |= provider.Update(runtime, context);
			}
			return updated;
		}
	}
}
