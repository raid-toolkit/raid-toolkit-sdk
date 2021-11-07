using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Il2CppToolkit.Runtime;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Raid.Service
{
    public class RaidInstance : IDisposable
    {
        public string Id;
        private Il2CsRuntimeContext Runtime;
        private Dictionary<IAccountFacet, object> FacetToValueMap;
        private UserAccount UserAccount;

        private readonly UserData UserData;
        private readonly StaticDataCache StaticDataCache;
        private readonly IReadOnlyList<IAccountFacet> Facets;
        private readonly ILogger<RaidInstance> Logger;

        public RaidInstance(UserData userData, StaticDataCache staticDataCache, IEnumerable<IAccountFacet> facets, ILogger<RaidInstance> logger)
        {
            UserData = userData;
            StaticDataCache = staticDataCache;
            Logger = logger;
            Facets = facets.ToList();
        }

        public RaidInstance Attach(Process process)
        {
            Runtime = new Il2CsRuntimeContext(process);
            Id = GetAccountId();
            UserAccount = UserData.GetAccount(Id);
            FacetToValueMap = Facets.ToDictionary(facet => facet, facet => facet.GetValue(UserAccount));

            // preload
            foreach (IFacet facet in FacetToValueMap.Keys)
            {
                facet.GetValue(UserAccount);
            }
            return this;
        }

        public void Update()
        {
            StaticDataCache.Update(Runtime);
            if (!StaticDataCache.IsReady)
            {
                return;
            }
            ModelScope scope = new(Runtime);
            foreach ((IAccountFacet facet, object currentValue) in FacetToValueMap)
            {
                string facetName = FacetAttribute.GetName(facet.GetType());
                try
                {
                    using var loggerScope = Logger.BeginScope(facet);

                    object newValue = facet.Merge(scope, currentValue);
                    FacetToValueMap[facet] = newValue;
                    if (newValue != currentValue)
                    {
                        if (JsonConvert.SerializeObject(newValue) != JsonConvert.SerializeObject(currentValue))
                        {
                            UserAccount.Set(facetName, newValue);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ServiceError.AccountUpdateFailed.EventId(), ex, $"Failed to update account facet '{facetName}'");
                }
            }
        }

        private string GetAccountId()
        {
            ModelScope scope = new(Runtime);
            var userWrapper = scope.AppModel._userWrapper;
            var socialWrapper = userWrapper.Social.SocialData;
            var globalId = socialWrapper.PlariumGlobalId;
            var socialId = socialWrapper.SocialId;
            return string.Join('_', globalId, socialId).Sha256();
        }

        public void Dispose()
        {
            Runtime.Dispose();
        }
    }
}