using System.Collections.Generic;
using Newtonsoft.Json;
using SharedModel.Meta.Battle.Arena;

namespace Raid.Service.DataModel
{
    public class ArenaData
    {
        [JsonProperty("league")]
        public ArenaLeagueId LeagueId;

        [JsonProperty("greatHallBonuses")]
        public IReadOnlyList<GreatHallBonus> GreatHallBonuses;
    }
}