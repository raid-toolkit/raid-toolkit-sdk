using System;
using System.Linq;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility.Providers;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.Extension.Account
{
    public class ResourcesProvider : DataProvider<AccountDataContext, Resources>
    {
        private static Version kVersion = new(2, 0);

        public override string Key => "resources";
        public override Version Version => kVersion;

        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        public ResourcesProvider(CachedDataStorage<PersistedDataStorage> storage)
        {
            Storage = storage;
        }

        public override bool Update(Il2CsRuntimeContext runtime, AccountDataContext context)
        {
            ModelScope scope = new(runtime);
            var userWrapper = scope.AppModel._userWrapper;
            var blackMarketItems = userWrapper.BlackMarket.BlackMarketData.Items;
            var shards = userWrapper.Shards.ShardData.Shards;
            var accountResources = userWrapper.Account.AccountData.Resources.RawValues;
            return Storage.Write(context, Key, new Resources
            {
                BlackMarket = blackMarketItems.ToDictionary(bmi => bmi.Key.ToString(), bmi => bmi.Value.Count),
                Shards = shards.ToDictionary(shard => shard.TypeId.ToString(), shard => shard.Count),
                Account = accountResources.ToDictionary(kvp => kvp.Key.ToString(), kvp => Math.Round(kvp.Value, 0))
            });
        }
    }
}
