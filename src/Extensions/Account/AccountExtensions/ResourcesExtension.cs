using Client.Model.Gameplay.Artifacts;
using StatKindId = SharedModel.Battle.Effects.StatKindId;

using Il2CppToolkit.Runtime;

using Microsoft.Extensions.Logging;
using Raid.Toolkit.Common;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extension.Account;

public class ResourcesExtension :
    AccountDataExtensionBase,
    IAccountPublicApi<IGetAccountDataApi<Resources>>,
    IGetAccountDataApi<Resources>,
    IAccountExportable
{
    private const string Key = "resources.json";

    IGetAccountDataApi<Resources> IAccountPublicApi<IGetAccountDataApi<Resources>>.GetApi() => this;
    bool IGetAccountDataApi<Resources>.TryGetData(out Resources data) => Storage.TryRead(Key, out data);

    public ResourcesExtension(IAccount account, IExtensionStorage storage, ILogger<ResourcesExtension> logger)
    : base(account, storage, logger)
    {
    }

    protected override Task Update(ModelScope scope)
    {
        var userWrapper = scope.AppModel._userWrapper;
        var blackMarketItems = userWrapper.BlackMarket.BlackMarketData.Items;
        var shards = userWrapper.Shards.ShardData.Shards;
        var accountResources = userWrapper.Account.AccountData.Resources.RawValues;
        Storage.Write(Key, new Resources
        {
            BlackMarket = blackMarketItems.ToDictionary(bmi => bmi.Key.ToString(), bmi => bmi.Value.Count),
            Shards = shards.ToDictionary(shard => shard.TypeId.ToString(), shard => shard.Count),
            Account = accountResources.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value)
        });
        return Task.CompletedTask;
    }

    public void Export(IAccountReaderWriter account)
    {
        if (Storage.TryRead(Key, out Resources data))
            account.Write(Key, data);
    }

    public void Import(IAccountReaderWriter account)
    {
        if (account.TryRead(Key, out Resources? data))
            Storage.Write(Key, data);
    }
}
