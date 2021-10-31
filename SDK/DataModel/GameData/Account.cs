using Newtonsoft.Json;

namespace Raid.DataModel
{
    public class Account
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("avatar")]
        public string Avatar;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("level")]
        public int Level;

        [JsonProperty("power")]
        public int Power;

        [JsonProperty("lastUpdated")]
        public string LastUpdated;
    }
}