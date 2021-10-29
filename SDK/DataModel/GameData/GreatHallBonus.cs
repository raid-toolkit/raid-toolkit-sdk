using System.Collections.Generic;
using Newtonsoft.Json;

namespace Raid.DataModel
{
    public class GreatHallBonus
    {
        [JsonProperty("affinity")]
        public string Affinity;

        [JsonProperty("bonus")]
        public IReadOnlyList<StatBonus> Bonus;

        [JsonProperty("levels")]
        public IReadOnlyDictionary<string, int> Levels;
    }
}