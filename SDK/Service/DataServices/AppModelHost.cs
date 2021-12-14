using Raid.Service;
using Raid.Service.DataServices;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class AppModelHostExtensions
    {
        public static IServiceCollection AddAppModel(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddModelHostShared()
                .AddSingleton<AccountDataBundle>()
                .AddSingleton(typeof(IPersistedDataManager<>), typeof(PersistedDataManager<>))
                .AddTypesAssignableTo<IContextDataProvider>(serviceCollection => serviceCollection.AddSingleton)
                .AddConcreteTypesAssignableTo<IContextDataProvider>(serviceCollection => serviceCollection.AddSingleton);
        }
    }
}