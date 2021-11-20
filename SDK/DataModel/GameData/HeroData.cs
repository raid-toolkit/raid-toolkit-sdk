using System.Collections.Generic;
using Newtonsoft.Json;

namespace Raid.DataModel
{
    public class HeroData
    {
        [JsonProperty("heroes")]
        public IReadOnlyDictionary<int, Hero> Heroes;

        [JsonProperty("stagePresets")]
        public IReadOnlyDictionary<int, int[]> BattlePresets;
    }
}