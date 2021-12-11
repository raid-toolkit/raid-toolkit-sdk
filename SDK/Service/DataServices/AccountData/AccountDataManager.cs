using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Il2CppToolkit.Runtime;
using Microsoft.Extensions.Logging;
using Raid.DataServices;

namespace Raid.Service.DataServices
{
    public class AccountDataManager
    {
        private readonly List<IAccountDataProvider> Providers;
        private readonly ILogger<AccountDataManager> Logger;

        public AccountDataManager(ILogger<AccountDataManager> logger, IEnumerable<IAccountDataProvider> providers)
        {
            Logger = logger;
            Providers = providers.ToList();
        }

        public void Upgrade()
        {
            throw new NotImplementedException();
        }

        public void Update(Il2CsRuntimeContext runtime, AccountDataContext context)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var results = Providers.AsParallel().Select(provider =>
            {
                Version facetVersion = DataProviderAttribute.GetVersion(provider.GetType());
                try
                {
                    using var loggerScope = Logger.BeginScope(provider);

                    ModelScope scope = new(runtime);
                    object newValue = provider.Update(scope, context);
                    FacetToValueMap[facet] = newValue;
                    if (newValue != currentValue && JsonConvert.SerializeObject(newValue) != JsonConvert.SerializeObject(currentValue))
                    {
                        Logger.LogInformation(ServiceEvent.DataUpdated.EventId(), $"Facet '{facet}' updated");
                        Set(facetName, newValue, facetVersion);
                        return UpdateResult.Updated;
                    }
                    return UpdateResult.NotUpdated;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ServiceError.AccountUpdateFailed.EventId(), ex, $"Failed to update account facet '{facetName}'");
                    return UpdateResult.Failed;
                }
            }).ToList();

            long end = sw.ElapsedMilliseconds;
            Logger.LogInformation($"Account update completed in {end}ms");

            if (results.Contains(UpdateResult.Updated))
            {
                FlushIndex();
                EventService.EmitAccountUpdated(UserId);
            }

            return !results.Contains(UpdateResult.Failed);
        }
    }
}
