using System.Collections.Generic;
using Newtonsoft.Json;

namespace Raid.Toolkit.DataModel
{
    public class Resources
    {
        [JsonProperty("shards")]
        public IReadOnlyDictionary<string, int> Shards;

        [JsonProperty("market")]
        public IReadOnlyDictionary<string, int> BlackMarket;

        [JsonProperty("basic")]
        public IReadOnlyDictionary<string, double> Account;
    }
}
