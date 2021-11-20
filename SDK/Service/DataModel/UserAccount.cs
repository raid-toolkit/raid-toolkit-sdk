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
        private Dictionary<string, object> Data = new();
        private string UserId;
        private UserData UserData;
        private Dictionary<string, AccountDataFacetInfo> FacetInfoIndex = new();
        private DateTime LastUpdatedAtRuntime = DateTime.UtcNow;

        private readonly IReadOnlyList<IAccountFacet> Facets;
        private Dictionary<IAccountFacet, object> FacetToValueMap;
        private readonly ILogger<UserAccount> Logger;

        public UserAccount(string userId, UserData userData, IServiceScope serviceScope)
        {
            UserData = userData;
            UserId = userId;
            Logger = serviceScope.ServiceProvider.GetService<ILogger<UserAccount>>();
            Facets = serviceScope.ServiceProvider.GetServices<IAccountFacet>().ToList();
            // TODO: avoid coupling with `this`?
            FacetToValueMap = Facets.ToDictionary(facet => facet, facet => facet.GetValue(this));

            // preload index
            var accountDataIndex = UserData.ReadAccountData<AccountDataIndex>(userId, "_index");
            FacetInfoIndex = accountDataIndex?.Facets != null ? new(accountDataIndex.Facets) : new();

            if (FacetInfoIndex.Count == 0)
                LastUpdatedAtRuntime = DateTime.UtcNow;
            else
                LastUpdatedAtRuntime = FacetInfoIndex.Values.Max(value => value.LastUpdated);
        }

        public void Upgrade()
        {

        }

        public void Update(ModelScope scope)
        {
            foreach ((IAccountFacet facet, object currentValue) in FacetToValueMap)
            {
                string facetName = FacetAttribute.GetName(facet.GetType());
                try
                {
                    using var loggerScope = Logger.BeginScope(facet);

                    object newValue = facet.Merge(scope, currentValue);
                    FacetToValueMap[facet] = newValue;
                    if (newValue != currentValue && JsonConvert.SerializeObject(newValue) != JsonConvert.SerializeObject(currentValue))
                    {
                        Logger.LogInformation(ServiceEvent.DataUpdated.EventId(), $"Facet '{facet}' updated");
                        Set(facetName, newValue);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ServiceError.AccountUpdateFailed.EventId(), ex, $"Failed to update account facet '{facetName}'");
                }
            }
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
