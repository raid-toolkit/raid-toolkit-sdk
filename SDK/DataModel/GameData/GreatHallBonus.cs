using System.Collections.Generic;
using Newtonsoft.Json;

namespace Raid.DataModel
{
    public class GreatHallBonus
    {
        [JsonProperty("affinity")]
        public Enums.Element Affinity;

        [JsonProperty("bonus")]
        public IReadOnlyList<StatBonus> Bonus;

        [JsonProperty("levels")]
        public IReadOnlyDictionary<Enums.StatKindId, int> Levels;
    }
}