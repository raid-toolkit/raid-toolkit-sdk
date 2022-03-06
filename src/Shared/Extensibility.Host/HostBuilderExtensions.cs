using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.Extensibility.Host.Services;
using Raid.Toolkit.Extensibility.HostInterfaces;
using Raid.Toolkit.Extensibility.Services;
using Raid.Toolkit.Extensibility.Shared;

namespace Raid.Toolkit.Extensibility
{
    public static class HostBuilderExtensions
    {
        public static IServiceCollection AddExtensibilityServices<TPackageManager>(this IServiceCollection services) where TPackageManager : class, IPackageManager
        {
            return (
                services
                .AddModelHostShared()
                .AddScoped<IPackageContext, PackageContext>()
                .AddSingleton<ExtensionHost>()
                .AddSingleton<IPackageInstanceFactory, PackageFactory>()
                .AddSingleton<IPackageLoader, SandboxedPackageLoader>()
                .AddSingleton<IContextDataManager, ContextDataManager>()
                .AddSingleton<IScopedServiceManager, ScopedServiceManager>()
                .AddSingleton<IProcessManager, ProcessManager>()
                .AddSingleton<IPackageManager, TPackageManager>()
                .AddSingleton<IGameInstanceManager, GameInstanceManager>()
                .AddSingleton<IExtensionHostController, ExtensionHost>()
                .AddSingleton(typeof(CachedDataStorage<>))
                .AddSingleton<PersistedDataStorage>()
                .AddHostedService<ApplicationHost>()
                .AddHostedServiceSingleton<ErrorService>()
                .AddHostedServiceSingleton<IDataStorageReaderWriter, FileStorageService>()
            );
        }
    }
}
