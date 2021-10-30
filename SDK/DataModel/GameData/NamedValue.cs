using Newtonsoft.Json;

namespace Raid.DataModel
{
    public class NamedValue
    {
        [JsonProperty("name")]
        public LocalizedText Name;
    }
}