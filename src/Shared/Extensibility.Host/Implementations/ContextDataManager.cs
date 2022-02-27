using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Extensibility.Providers;
using System;
using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility
{
	public class ContextDataManager : IContextDataManager
	{
		private readonly List<IContextDataProvider> ProvidersList = new();
		private readonly IServiceProvider ServiceProvider;

		public IReadOnlyList<IContextDataProvider> Providers => ProvidersList;

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
	}
}
