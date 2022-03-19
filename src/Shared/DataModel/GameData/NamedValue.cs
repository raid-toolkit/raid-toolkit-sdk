using Newtonsoft.Json;

namespace Raid.Toolkit.DataModel
{
    public class NamedValue
    {
        [JsonProperty("name")]
        public LocalizedText Name;
    }
}
