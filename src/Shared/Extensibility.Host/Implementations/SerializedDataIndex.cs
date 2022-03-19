using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Raid.Toolkit.Extensibility.Providers
{
    public class SerializedDataInfo
    {
        [JsonProperty("lastUpdated")]
        public DateTime LastUpdated;

        [JsonProperty("version")]
        public string Version;
    }

    public class SerializedDataIndex
    {
        [JsonProperty("lastUpdated")]
        public DateTime? LastUpdated
        {
            get => Facets?.Values.Max(value => value.LastUpdated);
            set { }
        }

        [JsonProperty("facets")]
        public IDictionary<string, SerializedDataInfo> Facets = new Dictionary<string, SerializedDataInfo>();
    }
}
