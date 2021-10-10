using System.Collections.Generic;
using Newtonsoft.Json;
using SharedModel.Meta.Account;
using SharedModel.Meta.BlackMarket;
using SharedModel.Meta.Shards;

namespace Raid.Service.DataModel
{
    public class Resources
    {
        [JsonProperty("shards")]
        public IReadOnlyDictionary<ShardTypeId, int> Shards;

        [JsonProperty("blackMarket")]
        public IReadOnlyDictionary<BlackMarketItemId, int> BlackMarket;

        [JsonProperty("account")]
        public IReadOnlyDictionary<ResourceTypeId, double> Account;
    }
}