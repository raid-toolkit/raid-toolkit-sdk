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
        private readonly IDataResolver<AccountDataContext, CachedDataStorage<PersistedDataStorage>, SerializedDataIndex> Index;

        public AccountDataManager(
            ILogger<AccountDataManager> logger, IEnumerable<IAccountDataProvider> providers,
            IDataResolver<AccountDataContext, CachedDataStorage<PersistedDataStorage>, SerializedDataIndex> index)
        {
            Logger = logger;
            Providers = providers.ToList();
            Index = index;
        }

        public void Upgrade(AccountDataContext context)
        {
            Logger.LogInformation($"Checking and upgrading account [{context.AccountId}]");
            if (!Index.TryRead(context, out SerializedDataIndex index))
            {
                index = new();
            }
            foreach (IAccountDataProvider provider in Providers)
            {
                var dataType = provider.DataType;
                string facetKey = dataType.Key;
                Version facetVersion = dataType.StructuredVersion;
                try
                {
                    using var loggerScope = Logger.BeginScope(provider);

                    // get version
                    Version dataVersion = new(1, 0);
                    if (index.Facets.TryGetValue(facetKey, out SerializedDataInfo facetInfo))
                    {
                        if (!string.IsNullOrEmpty(facetInfo.Version))
                            dataVersion = Version.Parse(facetInfo.Version);
                    }

                    if (dataVersion != facetVersion && provider.Upgrade(context, dataVersion))
                    {
                        Logger.LogInformation("Data upgraded");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ServiceError.AccountUpdateFailed.EventId(), ex, $"Failed to update account facet '{facetKey}'");
                }
            }
        }

        private enum UpdateResult
        {
            NotUpdated,
            Updated,
            Failed
        }

        public bool Update(Il2CsRuntimeContext runtime, AccountDataContext context)
        {
            Stopwatch sw = Stopwatch.StartNew();

            var results = Providers.AsParallel().Select(provider =>
            {
                var dataType = provider.DataType;
                string facetKey = dataType.Key;
                Version facetVersion = dataType.StructuredVersion;
                try
                {
                    using var loggerScope = Logger.BeginScope(provider);
                    if (provider.Update(new(runtime), context))
                    {
                        _ = Index.Update(context, index =>
                        {
                            index.Facets[facetKey] = new()
                            {
                                LastUpdated = DateTime.UtcNow,
                                Version = facetVersion.ToString()
                            };
                            return index;
                        });
                        return UpdateResult.Updated;
                    }
                    return provider.Update(new(runtime), context) ? UpdateResult.Updated : UpdateResult.NotUpdated;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ServiceError.AccountUpdateFailed.EventId(), ex, $"Failed to update account facet '{provider}'");
                    return UpdateResult.Failed;
                }
            }).ToList();

            Logger.LogInformation($"Account update completed in {sw.ElapsedMilliseconds}ms");

            return !results.Contains(UpdateResult.Failed);
        }
    }
}
