using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Il2CppToolkit.Runtime;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.Extensibility.DataServices;

namespace Raid.Toolkit.Extensibility.Providers
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
    }

    internal static class DataStorageExtensions
    {
        public static bool Update<TContext, TData>(this IDataStorage storage, TContext context, string key, Func<TData, TData> updateFn)
            where TContext : class, IDataContext
            where TData : class
        {
            _ = storage.TryRead(context, key, out TData value);
            return storage.Write(context, key, updateFn(value));
        }
    }

    public class PersistedDataManager<TContext> : IPersistedDataManager<TContext> where TContext : class, IDataContext
    {
        private readonly ILogger<PersistedDataManager<TContext>> Logger;
        private readonly IContextDataManager ContextDataManager;
        private IReadOnlyList<IDataProvider<TContext>> Providers => ContextDataManager.OfType<TContext>().ToList();
        private readonly CachedDataStorage<PersistedDataStorage> Storage;

        public PersistedDataManager(
            ILogger<PersistedDataManager<TContext>> logger, IContextDataManager contextDataManager,
            CachedDataStorage<PersistedDataStorage> storage)
        {
            Logger = logger;
            ContextDataManager = contextDataManager;
            Storage = storage;
        }

        public void Upgrade(TContext context)
        {
            using var upgradeScope = Logger.BeginScope(context);
            Logger.LogInformation($"Checking and upgrading context");
            if (!Storage.TryRead(context, "_index", out SerializedDataIndex index))
            {
                index = new();
            }
            foreach (IDataProvider<TContext> provider in Providers)
            {
                var dataType = provider.DataType;
                try
                {
                    using var loggerScope = Logger.BeginScope(provider);

                    // get version
                    Version dataVersion = new(1, 0);
                    if (index.Facets.TryGetValue(provider.Key, out SerializedDataInfo facetInfo))
                    {
                        if (!string.IsNullOrEmpty(facetInfo.Version))
                            dataVersion = Version.Parse(facetInfo.Version);
                    }

                    if (dataVersion != provider.Version && provider.Upgrade(context, dataVersion))
                    {
                        Logger.LogInformation("Data upgraded");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Failed to update facet '{provider.Key}'");
                }
            }
        }

        public UpdateResult Update(Il2CsRuntimeContext runtime, TContext context)
        {
            using var updateScope = Logger.BeginScope(context);
            Logger.LogInformation($"Starting update");

            Stopwatch sw = Stopwatch.StartNew();
            var results = Providers.AsParallel().Select(provider =>
            {
                var dataType = provider.DataType;
                try
                {
                    using var loggerScope = Logger.BeginScope(provider);

                    Stopwatch swScoped = Stopwatch.StartNew();
                    bool didUpdate = provider.Update(runtime, context);
                    Logger.LogInformation($"Provider '{provider.Key}' update completed in {swScoped.ElapsedMilliseconds}ms");

                    if (didUpdate)
                    {
                        _ = Storage.Update(context, "_index", (SerializedDataIndex index) =>
                        {
                            if (index == null)
                                index = new SerializedDataIndex();

                            index.Facets[provider.Key] = new()
                            {
                                LastUpdated = DateTime.UtcNow,
                                Version = provider.Version.ToString()
                            };
                            return index;
                        });
                        return UpdateResult.Updated;
                    }
                    return provider.Update(runtime, context) ? UpdateResult.Updated : UpdateResult.NotUpdated;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Failed to update facet '{provider}'");
                    return UpdateResult.Failed;
                }
            }).ToList();

            Logger.LogInformation($"Update completed in {sw.ElapsedMilliseconds}ms");

            return results.Contains(UpdateResult.Failed) ? UpdateResult.Failed : results.Contains(UpdateResult.Updated) ? UpdateResult.Updated : UpdateResult.NotUpdated;
        }
    }
}
