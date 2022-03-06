using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Raid.Toolkit.DataModel
{
    public class SerializedFacetInfo
    {
        [JsonProperty("lastUpdated")]
        public DateTime LastUpdated;

        [JsonProperty("version")]
        public string Version;
    }
    public class SerializedFacetIndex
    {
        [JsonProperty("lastUpdated")]
        public DateTime? LastUpdated
        {
            get => Facets?.Values.Max(value => value.LastUpdated);
            set { }
        }

        [JsonProperty("facets")]
        public IDictionary<string, SerializedFacetInfo> Facets = new Dictionary<string, SerializedFacetInfo>();
    }
}
