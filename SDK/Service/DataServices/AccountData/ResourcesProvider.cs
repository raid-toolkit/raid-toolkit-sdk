using System;
using System.Linq;
using Raid.DataModel;
using Raid.DataServices;

namespace Raid.Service.DataServices
{
    [DataType("Resources")]
    public class ResourcesDataObject : Resources
    {
    }

    public class ResourcesProvider : DataProviderBase<AccountDataContext, ResourcesDataObject>
    {
        public ResourcesProvider(
            IDataResolver<AccountDataContext, CachedDataStorage<PersistedDataStorage>, ResourcesDataObject> storage)
            : base(storage)
        {
        }

        public override bool Update(ModelScope scope, AccountDataContext context)
        {
            var userWrapper = scope.AppModel._userWrapper;
            var blackMarketItems = userWrapper.BlackMarket.BlackMarketData.Items;
            var shards = userWrapper.Shards.ShardData.Shards;
            var accountResources = userWrapper.Account.AccountData.Resources.RawValues;
            return PrimaryProvider.Write(context, new()
            {
                BlackMarket = blackMarketItems.ToDictionary(bmi => bmi.Key.ToString(), bmi => bmi.Value.Count),
                Shards = shards.ToDictionary(shard => shard.TypeId.ToString(), shard => shard.Count),
                Account = accountResources.ToDictionary(kvp => kvp.Key.ToString(), kvp => Math.Round(kvp.Value, 0))
            });
        }
    }
}
