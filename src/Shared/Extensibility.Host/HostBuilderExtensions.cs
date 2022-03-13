using System;
using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Model;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Host.Services;
using Raid.Toolkit.Extensibility.Providers;
using Raid.Toolkit.Extensibility.Services;

namespace Raid.Toolkit.Extensibility.Host
{
    [Flags]
    public enum HostFeatures
    {
        ProcessWatcher = (1 << 0),
        RefreshData = (1 << 1),
    }

    public static class HostBuilderExtensions
    {
        public static IServiceCollection AddFeatures(this IServiceCollection services, HostFeatures features)
        {
            if (features.HasFlag(HostFeatures.ProcessWatcher))
                services = services.AddHostedService<ProcessWatcherService>();
            if (features.HasFlag(HostFeatures.RefreshData))
                services = services.AddHostedService<RefreshDataService>();

            return services;
        }
        public static IServiceCollection AddExtensibilityServices<TPackageManager>(this IServiceCollection services) where TPackageManager : class, IPackageManager
        {
            return (
                services
                .AddScoped<IPackageContext, PackageContext>()
                .AddSingleton<ExtensionHost>()
                .AddSingleton<IModelLoader, ModelLoader>()
                .AddSingleton<IPackageLoader, SandboxedPackageLoader>()
                .AddSingleton<IPackageInstanceFactory, PackageFactory>()
                .AddSingleton<IContextDataManager, ContextDataManager>()
                .AddSingleton<IScopedServiceManager, ScopedServiceManager>()
                .AddSingleton<ISessionManager, SessionManager>()
                .AddSingleton<IProcessManager, ProcessManager>()
                .AddSingleton<IPackageManager, TPackageManager>()
                .AddSingleton<IGameInstanceManager, GameInstanceManager>()
                .AddSingleton<IExtensionHostController, ExtensionHost>()
                .AddSingleton(typeof(CachedDataStorage))
                .AddSingleton(typeof(CachedDataStorage<>))
                .AddSingleton(typeof(PersistedDataManager<>))
                .AddSingleton<PersistedDataStorage>()
                .AddSingleton<ErrorService>()
                .AddSingleton<GitHub.Updater>()
                .AddHostedServiceSingleton<UpdateService>()
                .AddHostedService<ApplicationHost>()
                .AddHostedServiceSingleton<IDataStorageReaderWriter, FileStorageService>()
            );
        }
    }
}
