using Newtonsoft.Json;
using SharedModel.Battle.Effects;

namespace Raid.Service.DataModel
{
    public class StatBonus
    {
        [JsonProperty("kind")]
        public StatKindId KindId;

        [JsonProperty("absolute")]
        public bool Absolute;

        [JsonProperty("value")]
        public float Value;
    }
}