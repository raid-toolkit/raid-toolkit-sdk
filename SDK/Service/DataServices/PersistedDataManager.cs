using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Il2CppToolkit.Runtime;
using Microsoft.Extensions.Logging;
using Raid.DataServices;

namespace Raid.Service.DataServices
{
    public enum UpdateResult
    {
        NotUpdated,
        Updated,
        Failed
    }

    public interface IPersistedDataManager<TContext> where TContext : class, IDataContext
    {
        void Upgrade(TContext context);
        UpdateResult Update(Il2CsRuntimeContext runtime, TContext context);
        IDataResolver<TContext, CachedDataStorage<PersistedDataStorage>, SerializedDataIndex> Index { get; }
    }

    public class PersistedDataManager<TContext> : IPersistedDataManager<TContext> where TContext : class, IDataContext
    {
        private readonly List<IContextDataProvider<TContext>> Providers;
        private readonly ILogger<PersistedDataManager<TContext>> Logger;
        public IDataResolver<TContext, CachedDataStorage<PersistedDataStorage>, SerializedDataIndex> Index { get; }

        public PersistedDataManager(
            ILogger<PersistedDataManager<TContext>> logger, IEnumerable<IContextDataProvider> providers,
            IDataResolver<TContext, CachedDataStorage<PersistedDataStorage>, SerializedDataIndex> index)
        {
            Logger = logger;
            Providers = providers.OfType<IContextDataProvider<TContext>>().ToList();
            Index = index;
        }

        public void Upgrade(TContext context)
        {
            Logger.LogInformation($"Checking and upgrading context [{string.Join(',', context.Parts)}]");
            if (!Index.TryRead(context, out SerializedDataIndex index))
            {
                index = new();
            }
            foreach (IContextDataProvider<TContext> provider in Providers)
            {
                var dataType = provider.DataType;
                try
                {
                    using var loggerScope = Logger.BeginScope(provider);

                    // get version
                    Version dataVersion = new(1, 0);
                    if (index.Facets.TryGetValue(dataType.Key, out SerializedDataInfo facetInfo))
                    {
                        if (!string.IsNullOrEmpty(facetInfo.Version))
                            dataVersion = Version.Parse(facetInfo.Version);
                    }

                    if (dataVersion != dataType.StructuredVersion && provider.Upgrade(context, dataVersion))
                    {
                        Logger.LogInformation("Data upgraded");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ServiceError.AccountUpdateFailed.EventId(), ex, $"Failed to update facet '{dataType.Key}'");
                }
            }
        }

        public UpdateResult Update(Il2CsRuntimeContext runtime, TContext context)
        {
            Stopwatch sw = Stopwatch.StartNew();

            var results = Providers.AsParallel().Select(provider =>
            {
                var dataType = provider.DataType;
                try
                {
                    using var loggerScope = Logger.BeginScope(provider);
                    if (provider.Update(new(runtime), context))
                    {
                        _ = Index.Update(context, index =>
                        {
                            if (index == null)
                                index = new SerializedDataIndex();

                            index.Facets[dataType.Key] = new()
                            {
                                LastUpdated = DateTime.UtcNow,
                                Version = dataType.StructuredVersion.ToString()
                            };
                            return index;
                        });
                        return UpdateResult.Updated;
                    }
                    return provider.Update(new(runtime), context) ? UpdateResult.Updated : UpdateResult.NotUpdated;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ServiceError.AccountUpdateFailed.EventId(), ex, $"Failed to update facet '{provider}'");
                    return UpdateResult.Failed;
                }
            }).ToList();

            Logger.LogInformation($"Update completed in {sw.ElapsedMilliseconds}ms");

            return results.Contains(UpdateResult.Failed) ? UpdateResult.Failed : results.Contains(UpdateResult.Updated) ? UpdateResult.Updated : UpdateResult.NotUpdated;
        }
    }
}
