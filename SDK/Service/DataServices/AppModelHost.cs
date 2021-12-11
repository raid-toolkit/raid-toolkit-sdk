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
                .AddSingleton<AccountDataManager>()
                .AddTypesAssignableToFactories<IAccountDataProvider>(serviceCollection => serviceCollection.AddSingleton);
        }
    }
}