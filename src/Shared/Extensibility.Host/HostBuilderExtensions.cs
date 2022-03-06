using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.Extensibility.Services;

namespace Raid.Toolkit.Extensibility
{
    public static class HostBuilderExtensions
    {
        public static IServiceCollection AddExtensibilityServices<TPackageManager>(this IServiceCollection services) where TPackageManager : class, IPackageManager
        {
            return services
                .AddModelHostShared()
                .AddScoped<IPackageContext, PackageContext>()
                .AddSingleton<ExtensionHost>()
                .AddSingleton<IPackageInstanceFactory, PackageFactory>()
                .AddSingleton<IPackageLoader, SandboxedPackageLoader>()
                .AddSingleton<IContextDataManager, ContextDataManager>()
                .AddSingleton<IScopedServiceManager, ScopedServiceManager>()
                .AddSingleton<IPackageManager, TPackageManager>()
                .AddSingleton(typeof(CachedDataStorage<>))
                .AddSingleton<PersistedDataStorage>()
                .AddHostedServiceSingleton<IDataStorageReaderWriter, FileStorage>();
        }
    }
}
