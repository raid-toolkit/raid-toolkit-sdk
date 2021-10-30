using System.Collections.Generic;
using Newtonsoft.Json;

namespace Raid.DataModel
{
    public class ArenaData
    {
        [JsonProperty("league")]
        public string LeagueId;

        [JsonProperty("greatHallBonuses")]
        public IReadOnlyList<GreatHallBonus> GreatHallBonuses;
    }
}