using System;
using System.Collections.Generic;
using System.Linq;
using Raid.DataModel;

namespace Raid.Service
{
    public class UserAccount : IModelDataSource
    {
        private Dictionary<string, object> Data = new();
        private string UserId;
        private UserData UserData;
        private Dictionary<string, AccountDataFacetInfo> FacetInfoIndex = new();
        private DateTime LastUpdatedAtRuntime = DateTime.UtcNow;

        public UserAccount(string userId, UserData userData)
        {
            UserData = userData;
            UserId = userId;

            // preload index
            var accountDataIndex = UserData.ReadAccountData<AccountDataIndex>(userId, "_index");
            FacetInfoIndex = accountDataIndex?.Facets != null ? new(accountDataIndex.Facets) : new();

            if (FacetInfoIndex.Count == 0)
                LastUpdatedAtRuntime = DateTime.UtcNow;
            else
                LastUpdatedAtRuntime = FacetInfoIndex.Values.Max(value => value.LastUpdated);
        }

        public T Get<T>(string key) where T : class
        {
            if (!Data.TryGetValue(key, out object value))
            {
                value = UserData.ReadAccountData<T>(UserId, key);
                Data.Add(key, value);
            }
            return (T)value;
        }

        public void Set<T>(string key, T value) where T : class
        {
            LastUpdatedAtRuntime = DateTime.UtcNow;
            Data[key] = value;
            UserData.WriteAccountData(UserId, key, value);

            // update index
            FacetInfoIndex[key] = new AccountDataFacetInfo() { LastUpdated = DateTime.UtcNow };
            UserData.WriteAccountData(UserId, "_index", new AccountDataIndex() { Facets = FacetInfoIndex });
        }

        public DateTime LastUpdated => LastUpdatedAtRuntime;
    }
}
