using System;
using System.Collections.Generic;
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
        private Dictionary<IAccountFacet, object> FacetToValueMap;
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
            FacetToValueMap = Facets.ToDictionary(facet => facet, facet => facet.GetValue(this));
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

        public bool Update(ModelScope scope)
        {
            bool success = true;
            bool updated = false;
            foreach ((IAccountFacet facet, object currentValue) in FacetToValueMap)
            {
                string facetName = FacetAttribute.GetName(facet.GetType());
                Version facetVersion = FacetAttribute.GetVersion(facet.GetType());
                try
                {
                    using var loggerScope = Logger.BeginScope(facet);

                    object newValue = facet.Merge(scope, currentValue);
                    FacetToValueMap[facet] = newValue;
                    if (newValue != currentValue && JsonConvert.SerializeObject(newValue) != JsonConvert.SerializeObject(currentValue))
                    {
                        updated = true;
                        Logger.LogInformation(ServiceEvent.DataUpdated.EventId(), $"Facet '{facet}' updated");
                        Set(facetName, newValue, facetVersion);
                    }
                }
                catch (Exception ex)
                {
                    success = false;
                    Logger.LogError(ServiceError.AccountUpdateFailed.EventId(), ex, $"Failed to update account facet '{facetName}'");
                }
            }

            if (updated)
                EventService.EmitAccountUpdated(UserId);

            return success;
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
            UserData.WriteAccountData(UserId, "_index", Index);
        }

        public bool TryRead<T>(string key, out T value) where T : class
        {
            throw new NotImplementedException();
        }

        public DateTime LastUpdated { get; private set; } = DateTime.UtcNow;
    }
}
