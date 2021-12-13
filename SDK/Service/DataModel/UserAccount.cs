using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Raid.DataModel;
using Raid.Service.DataServices;

namespace Raid.Service
{
    public class UserAccount : IModelDataSource
    {
        private readonly Dictionary<string, object> Data = new();
        private readonly string UserId;
        private readonly AppData UserData;
        private readonly IReadOnlyList<IAccountFacet> Facets;
        private ConcurrentDictionary<IAccountFacet, object> FacetToValueMap;
        private readonly ILogger<UserAccount> Logger;
        private readonly SerializedFacetIndex Index;
        private readonly EventService EventService;
        private readonly IPersistedDataManager<AccountDataContext> DataManager;

        public UserAccount(string userId, AppData userData, IServiceScope serviceScope)
        {
            UserData = userData;
            UserId = userId;
            Logger = serviceScope.ServiceProvider.GetService<ILogger<UserAccount>>();
            Facets = serviceScope.ServiceProvider.GetServices<IAccountFacet>().ToList();
            EventService = serviceScope.ServiceProvider.GetRequiredService<EventService>();
            DataManager = serviceScope.ServiceProvider.GetRequiredService<IPersistedDataManager<AccountDataContext>>();

            // preload index
            Index = UserData.ReadAccountData<SerializedFacetIndex>(userId, "_index") ?? new();
            LastUpdated = Index.Facets.Count == 0 ? DateTime.UtcNow : Index.Facets.Values.Max(value => value.LastUpdated);
        }

        public void Load()
        {
            Upgrade();
            FacetToValueMap = new(Facets.ToDictionary(facet => facet, facet => facet.GetValue(this)));
        }

        private void Upgrade()
        {
            DataManager.Upgrade(new AccountDataContext(UserId));
        }

        public bool Update(Il2CppToolkit.Runtime.Il2CsRuntimeContext runtime)
        {
            var updateResult = DataManager.Update(runtime, new AccountDataContext(UserId));
            if (updateResult == UpdateResult.Updated)
            {
                EventService.EmitAccountUpdated(UserId);
            }
            return updateResult != UpdateResult.Failed;
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
