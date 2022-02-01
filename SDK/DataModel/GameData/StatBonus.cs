using Newtonsoft.Json;

namespace Raid.DataModel
{
    public class StatBonus
    {
        [JsonProperty("kind")]
        public string KindId;

        [JsonProperty("absolute")]
        public bool Absolute;

        [JsonProperty("value")]
        public double Value;
    }
}
