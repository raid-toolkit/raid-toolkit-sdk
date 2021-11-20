using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Raid.DataModel
{
    public class AccountDataFacetInfo
    {
        [JsonProperty("lastUpdated")]
        public DateTime LastUpdated;

        [JsonProperty("version")]
        public int[] Version;
    }
    public class AccountDataIndex
    {
        [JsonProperty("lastUpdated")]
        public DateTime? LastUpdated
        {
            get => Facets?.Values.Max(value => value.LastUpdated);
            set { }
        }

        [JsonProperty("facets")]
        public IReadOnlyDictionary<string, AccountDataFacetInfo> Facets;
    }
}