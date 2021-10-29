using System.Collections.Generic;
using Newtonsoft.Json;

namespace Raid.DataModel
{
    public class LocalizedText
    {
        [JsonProperty("key")]
        public string Key;

        [JsonProperty("defaultValue")]
        public string DefaultValue;

        [JsonProperty("EnValue")]
        public string EnValue;
    }
}