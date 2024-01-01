using System;

using Microsoft.Extensions.DependencyInjection;

using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Host.Services;
using Raid.Toolkit.Extensibility.Implementations;
using Raid.Toolkit.Extensibility.Interfaces;
using Raid.Toolkit.Extensibility.Notifications;
using Raid.Toolkit.Loader;

namespace Raid.Toolkit.Extensibility.Host
{
	[Flags]
	public enum HostFeatures
	{
		ProcessWatcher = 1 << 0,
		AutoUpdate = 1 << 2
	}

	public enum ServiceMode
	{
		Worker,
		Server
	}

	public static class HostBuilderExtensions
	{
		public static IServiceCollection AddFeatures(this IServiceCollection services, HostFeatures features)
		{
			if (features.HasFlag(HostFeatures.ProcessWatcher))
				services = services.AddHostedService<ProcessWatcherService>();
			services = features.HasFlag(HostFeatures.AutoUpdate)
				? services.AddHostedServiceSingleton<IUpdateService, UpdateService>()
				: services.AddHostedServiceSingleton<IUpdateService, UpdateServiceStub>();

			return services;
		}
		public static IServiceCollection AddExtensibilityServices<TPackageManager>(this IServiceCollection services, ServiceMode mode) where TPackageManager : class, IPackageManager
		{
			switch (mode)
			{
				case ServiceMode.Server:
					services.AddHostedServiceSingleton<IServerApplication, ServerApplication>()
						.AddSingleton<IMenuManager, MenuManager>();
					break;
				case ServiceMode.Worker:
					services.AddHostedServiceSingleton<IWorkerApplication, WorkerApplication>()
						.AddSingleton<IMenuManager, MenuManager>();
					break;
			}
			return services
				.AddSingleton<ManagedPackage>()
				.AddSingleton<IModelLoader, ModelLoader>()
				.AddSingleton<IPackageLoader, SandboxedPackageLoader>()
				.AddSingleton<IManagedPackageFactory, ManagedPackageFactory>()
				.AddSingleton<IServiceManager, ServiceManager>()
				.AddSingleton<IProcessManager, ProcessManager>()
				.AddSingleton<IWindowManager, WindowManager>()
				.AddSingleton<IPackageManager, TPackageManager>()
				.AddSingleton<IGameInstanceManager, GameInstanceManager>()
				.AddSingleton<IPackageWorkerManager, PackageWorkerManager>()
				.AddSingleton<IAppDispatcher, AppDispatcher>()
				.AddSingleton(typeof(CachedDataStorage))
				.AddSingleton(typeof(CachedDataStorage<>))
				.AddSingleton<PersistedDataStorage>()
				.AddSingleton<ErrorService>()
				.AddSingleton<GitHub.Updater>()
				.AddHostedService<ServiceExecutor>()
				.AddHostedServiceSingleton<IAccountManager, AccountManager>()
				.AddHostedServiceSingleton<INotificationManager, NotificationManager>()
				.AddHostedServiceSingleton<IDataStorageReaderWriter, FileStorageService>();
		}
	}
}
