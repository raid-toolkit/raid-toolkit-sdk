using Newtonsoft.Json;

namespace Raid.Toolkit.DataModel
{
    public class ArenaLeague
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("statBonus")]
        public Stats StatBonus;
    }
}
