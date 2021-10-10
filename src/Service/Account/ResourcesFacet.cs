using System.Linq;
using Raid.Service.DataModel;

namespace Raid.Service
{
    public class ResourcesFacet : Facet<Resources>
    {
        public override string Id => "resources";

        protected override Resources Merge(ModelScope scope, Resources previous = null)
        {
            var userWrapper = scope.AppModel._userWrapper;
            var blackMarketItems = userWrapper.BlackMarket.BlackMarketData.Items;
            var shards = userWrapper.Shards.ShardData.Shards;
            var accountResources = userWrapper.Account.AccountData.Resources.RawValues;
            return new Resources
            {
                BlackMarket = blackMarketItems.ToDictionary(bmi => bmi.Key, bmi => bmi.Value.Count),
                Shards = shards.ToDictionary(shard => shard.TypeId, shard => shard.Count),
                Account = accountResources.UnderlyingDictionary
            };
        }
    }
}