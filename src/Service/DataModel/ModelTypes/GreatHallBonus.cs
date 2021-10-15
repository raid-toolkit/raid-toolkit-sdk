using System.Collections.Generic;
using Newtonsoft.Json;
using SharedModel.Battle.Effects;
using SharedModel.Meta.Heroes;

namespace Raid.Service.DataModel
{
    public class GreatHallBonus
    {
        [JsonProperty("affinity")]
        public Element Affinity;

        [JsonProperty("bonus")]
        public IReadOnlyList<StatBonus> Bonus;

        [JsonProperty("levels")]
        public IReadOnlyDictionary<StatKindId, int> Levels;
    }
}