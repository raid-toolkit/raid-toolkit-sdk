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
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IReadOnlyDictionary<int, int> _Shards
        {
            get { return Shards?.ToDictionary(kvp => (int)kvp.Key, kvp => kvp.Value); }
            set { Shards = value?.ToDictionary(kvp => (ShardTypeId)kvp.Key, kvp => kvp.Value); }
        }

        [JsonIgnore]
        public IReadOnlyDictionary<ShardTypeId, int> Shards;

        [JsonProperty("market")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IReadOnlyDictionary<int, int> _BlackMarket
        {
            get { return BlackMarket?.ToDictionary(kvp => (int)kvp.Key, kvp => kvp.Value); }
            set { BlackMarket = value?.ToDictionary(kvp => (BlackMarketItemId)kvp.Key, kvp => kvp.Value); }
        }

        [JsonIgnore]
        public IReadOnlyDictionary<BlackMarketItemId, int> BlackMarket;

        [JsonProperty("basic")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IReadOnlyDictionary<int, double> _Account
        {
            get { return Account?.ToDictionary(kvp => (int)kvp.Key, kvp => kvp.Value); }
            set { Account = value?.ToDictionary(kvp => (ResourceTypeId)kvp.Key, kvp => kvp.Value); }
        }

        [JsonIgnore]
        public IReadOnlyDictionary<ResourceTypeId, double> Account;
    }
}