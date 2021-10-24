using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Il2CppToolkit.Runtime;
using Raid.Service.DataModel;

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

        public RaidInstance(UserData userData, StaticDataCache staticDataCache, IEnumerable<IAccountFacet> facets)
        {
            UserData = userData;
            StaticDataCache = staticDataCache;
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
                object newValue = facet.Merge(scope, currentValue);
                FacetToValueMap[facet] = newValue;
                if (newValue != currentValue)
                {
                    UserAccount.Set(FacetAttribute.GetName(facet.GetType()), newValue);
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