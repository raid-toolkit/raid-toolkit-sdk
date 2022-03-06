using Newtonsoft.Json;

namespace Raid.Toolkit.DataModel
{
    public class LocalizedText
    {
        [JsonProperty("key")]
        public string Key;

        [JsonProperty("defaultValue")]
        public string DefaultValue;

        [JsonProperty("localizedValue")]
        public string LocalizedValue;
    }
}
