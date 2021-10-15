using Newtonsoft.Json;
using SharedModel.Meta.Battle.Arena;

namespace Raid.Service.DataModel
{
    public class ArenaLeague
    {
        [JsonProperty("id")]
        public ArenaLeagueId Id;

        [JsonProperty("statBonus")]
        public Stats StatBonus;
    }

    public static partial class ModelExtensions
    {
        public static ArenaLeague ToModel(this ArenaLeagueInfo arenaLeague)
        {
            return new()
            {
                Id = arenaLeague.Id,
                StatBonus = arenaLeague.BattleBonuses.ToModel(),
            };
        }
    }
}