using System;

using Microsoft.Extensions.DependencyInjection;

using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Host.Services;
using Raid.Toolkit.Extensibility.Notifications;
using Raid.Toolkit.Extensibility.Services;
using Raid.Toolkit.Loader;

namespace Raid.Toolkit.Extensibility.Host
{
    [Flags]
    public enum HostFeatures
    {
        ProcessWatcher = 1 << 0,
        AutoUpdate = 1 << 2
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
        public static IServiceCollection AddExtensibilityServices<TPackageManager>(this IServiceCollection services) where TPackageManager : class, IPackageManager
        {
            return
                services
                .AddSingleton<ExtensionHost>()
                .AddSingleton<IModelLoader, ModelLoader>()
                .AddSingleton<IPackageLoader, SandboxedPackageLoader>()
                .AddSingleton<IPackageInstanceFactory, PackageFactory>()
                .AddSingleton<IScopedServiceManager, ScopedServiceManager>()
                .AddSingleton<ISessionManager, SessionManager>()
                .AddSingleton<IServiceManager, ServiceManager>()
                .AddSingleton<IProcessManager, ProcessManager>()
                .AddSingleton<IMenuManager, MenuManager>()
                .AddSingleton<IWindowManager, WindowManager>()
                .AddSingleton<IPackageManager, TPackageManager>()
                .AddSingleton<IGameInstanceManager, GameInstanceManager>()
                .AddSingleton<IExtensionHostController, ExtensionHostController>()
                .AddSingleton(typeof(CachedDataStorage))
                .AddSingleton(typeof(CachedDataStorage<>))
                .AddSingleton<PersistedDataStorage>()
                .AddSingleton<ErrorService>()
                .AddSingleton<GitHub.Updater>()
                .AddHostedService<ServiceExecutor>()
                .AddHostedServiceSingleton<IAccountManager, AccountManager>()
                .AddHostedServiceSingleton<INotificationManager, NotificationManager>()
                .AddHostedServiceSingleton<IDataStorageReaderWriter, FileStorageService>()
            ;
        }
    }
}
