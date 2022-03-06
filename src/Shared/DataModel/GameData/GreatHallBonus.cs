using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Raid.Toolkit.DataModel
{
    public class GreatHallBonus
    {
        [JsonProperty("affinity"), JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(CamelCaseNamingStrategy))]
        public Enums.Element Affinity;

        [JsonProperty("bonus")]
        public IReadOnlyList<StatBonus> Bonus;

        [JsonProperty("levels")]
        public IReadOnlyDictionary<Enums.StatKindId, int> Levels;
    }
}
