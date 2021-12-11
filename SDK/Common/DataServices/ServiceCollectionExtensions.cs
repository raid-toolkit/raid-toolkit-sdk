using Raid.DataServices;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class AppModelHostExtensions
    {
        public static IServiceCollection AddModelHostShared(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton(typeof(IDataStorageFactory<>), typeof(DataStorageFactoryManager<>))
                .AddSingleton(typeof(IDataType<>), typeof(DataTypeManager<>))
                .AddScoped(typeof(IDataResolver<,,>), typeof(DataResolverManager<,,>))
                .AddScoped(typeof(IDataType<>), typeof(DataTypeManager<>))
            ;
        }
    }
}
