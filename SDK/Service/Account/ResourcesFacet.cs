using System;
using System.Linq;
using Raid.DataModel;

namespace Raid.Service
{
    [Facet("resources")]
    public class ResourcesFacet : UserAccountFacetBase<Resources, ResourcesFacet>
    {
        protected override Resources Merge(ModelScope scope, Resources previous = null)
        {
            var userWrapper = scope.AppModel._userWrapper;
            var blackMarketItems = userWrapper.BlackMarket.BlackMarketData.Items;
            var shards = userWrapper.Shards.ShardData.Shards;
            var accountResources = userWrapper.Account.AccountData.Resources.RawValues;
            return new Resources
            {
                BlackMarket = blackMarketItems.ToDictionary(bmi => bmi.Key.ToString(), bmi => bmi.Value.Count),
                Shards = shards.ToDictionary(shard => shard.TypeId.ToString(), shard => shard.Count),
                Account = accountResources.ToDictionary(kvp => kvp.Key.ToString(), kvp => Math.Round(kvp.Value, 0))
            };
        }
    }
}