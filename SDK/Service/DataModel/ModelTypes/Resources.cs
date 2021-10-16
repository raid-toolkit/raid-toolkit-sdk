using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using SharedModel.Meta.Account;
using SharedModel.Meta.BlackMarket;
using SharedModel.Meta.Shards;

namespace Raid.Service.DataModel
{
    public class Resources
    {
        [JsonProperty("shards")]
        public NumericDictionary<ShardTypeId, int> Shards;

        [JsonProperty("market")]
        public NumericDictionary<BlackMarketItemId, int> BlackMarket;

        [JsonProperty("basic")]
        public NumericDictionary<ResourceTypeId, double> Account;
    }
}