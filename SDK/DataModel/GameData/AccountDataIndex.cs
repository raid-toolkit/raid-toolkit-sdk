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
        public string Version;
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
        public IDictionary<string, AccountDataFacetInfo> Facets = new Dictionary<string, AccountDataFacetInfo>();
    }
}