using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Raid.DataModel;

namespace Raid.Service
{
    public class UserAccount : IModelDataSource
    {
        private readonly Dictionary<string, object> Data = new();
        private readonly string UserId;
        private readonly UserData UserData;
        private readonly IReadOnlyList<IAccountFacet> Facets;
        private ConcurrentDictionary<IAccountFacet, object> FacetToValueMap;
        private readonly ILogger<UserAccount> Logger;
        private readonly SerializedDataIndex Index;
        private readonly EventService EventService;

        public UserAccount(string userId, UserData userData, IServiceScope serviceScope)
        {
            UserData = userData;
            UserId = userId;
            Logger = serviceScope.ServiceProvider.GetService<ILogger<UserAccount>>();
            Facets = serviceScope.ServiceProvider.GetServices<IAccountFacet>().ToList();
            EventService = serviceScope.ServiceProvider.GetRequiredService<EventService>();

            // preload index
            Index = UserData.ReadAccountData<SerializedDataIndex>(userId, "_index") ?? new();
            LastUpdated = Index.Facets.Count == 0 ? DateTime.UtcNow : Index.Facets.Values.Max(value => value.LastUpdated);
        }

        public void Load()
        {
            Upgrade();
            FacetToValueMap = new(Facets.ToDictionary(facet => facet, facet => facet.GetValue(this)));
        }

        private void Upgrade()
        {
            Logger.LogInformation($"Checking and upgrading account [${UserId}]");
            foreach (IAccountFacet facet in Facets)
            {
                string facetName = FacetAttribute.GetName(facet.GetType());
                Version facetVersion = FacetAttribute.GetVersion(facet.GetType());
                try
                {
                    using var loggerScope = Logger.BeginScope(facet);

                    // get version
                    Version dataVersion = new(1, 0);
                    if (Index.Facets.TryGetValue(facetName, out SerializedFacetInfo facetInfo))
                    {
                        if (!string.IsNullOrEmpty(facetInfo.Version))
                            dataVersion = Version.Parse(facetInfo.Version);
                    }

                    if (dataVersion != facetVersion && facet.TryUpgrade(this, dataVersion, out object upgradedData))
                    {
                        Set(facetName, upgradedData, facetVersion);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ServiceError.AccountUpdateFailed.EventId(), ex, $"Failed to update account facet '{facetName}'");
                }
            }
        }

        private enum UpdateResult
        {
            NotUpdated,
            Updated,
            Failed
        }

        public bool Update(Il2CppToolkit.Runtime.Il2CsRuntimeContext runtime)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var results = FacetToValueMap.AsParallel().Select((kvp, _) =>
            {
                IAccountFacet facet = kvp.Key;
                object currentValue = kvp.Value;
                string facetName = FacetAttribute.GetName(facet.GetType());
                Version facetVersion = FacetAttribute.GetVersion(facet.GetType());
                try
                {
                    using var loggerScope = Logger.BeginScope(facet);

                    ModelScope scope = new(runtime);
                    object newValue = facet.Merge(scope, currentValue);
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

        public T Read<T>(string key) where T : class
        {
            return UserData.ReadAccountData<T>(UserId, key);
        }

        public T Get<T>(string key) where T : class
        {
            if (!Data.TryGetValue(key, out object value))
            {
                value = Read<T>(key);
                Data.Add(key, value);
            }
            return (T)value;
        }

        public void Set<T>(string key, T value, Version version) where T : class
        {
            LastUpdated = DateTime.UtcNow;
            Data[key] = value;
            UserData.WriteAccountData(UserId, key, value);

            // update index
            if (!Index.Facets.TryGetValue(key, out var facetInfo))
            {
                Index.Facets[key] = facetInfo = new SerializedFacetInfo();
            }
            facetInfo.LastUpdated = DateTime.UtcNow;
            facetInfo.Version = version.ToString();
        }

        public void FlushIndex()
        {
            UserData.WriteAccountData(UserId, "_index", Index);
        }

        public DateTime LastUpdated { get; private set; } = DateTime.UtcNow;
    }
}
