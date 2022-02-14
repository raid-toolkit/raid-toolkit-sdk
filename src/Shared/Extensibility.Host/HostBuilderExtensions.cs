using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Extensibility.Host;

namespace Raid.Toolkit.Extensibility
{
    public static class HostBuilderExtensions
    {
        public static IServiceCollection AddExtensibilityServices<TPackageManager>(this IServiceCollection services) where TPackageManager : class, IPackageManager
        {
            return services
                .AddScoped<IPackageContext, PackageContext>()
                .AddSingleton<ExtensionHost>()
                .AddSingleton<IPackageInstanceFactory, PackageFactory>()
                .AddSingleton<IPackageLoader, SandboxedPackageLoader>()
                .AddSingleton<IPackageManager, TPackageManager>();
        }
    }
}
