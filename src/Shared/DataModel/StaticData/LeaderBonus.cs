using Newtonsoft.Json;

namespace Raid.Toolkit.DataModel
{
    public class LeaderStatBonus : StatBonus
    {
        [JsonProperty("area")]
        public string AreaTypeId;

        [JsonProperty("affinity")]
        public string Affinity;
    }
}
