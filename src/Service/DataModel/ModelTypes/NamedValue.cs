using Newtonsoft.Json;

namespace Raid.Service.DataModel
{
    public class NamedValue
    {
        [JsonProperty("name")]
        public LocalizedText Name;
    }
}