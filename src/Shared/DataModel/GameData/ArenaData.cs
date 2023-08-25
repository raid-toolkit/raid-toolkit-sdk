using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Raid.Toolkit.DataModel.Enums;

namespace Raid.Toolkit.DataModel
{
    public class ArenaData
    {
        [JsonProperty("greatHallBonuses")]
        public IReadOnlyList<GreatHallBonus> GreatHallBonuses;

        [JsonProperty("classic")]
        public ClassicArenaData ClassicArena;

        [JsonProperty("tag")]
        public TagArenaData TagArena;
    }

    public class SharedArenaData
    {
        [JsonProperty("league"), JsonConverter(typeof(StringEnumConverter))]
        public ArenaLeagueId? LeagueId;

        [JsonProperty("points")]
        public long ArenaPoints;

        [JsonProperty("weeklyStats")]
        public WinLossRatio WeeklyStats;

    }

    public enum TagArenaPlacement
    {
        Unknown = 0,
        Demotion,
        Retain,
        Promotion,
    }

    public class TagArenaData : SharedArenaData
    {
        [JsonProperty("defenseHeroIds")]
        public int[][] DefenseHeroIds;

        [JsonProperty("placement"), JsonConverter(typeof(StringEnumConverter))]
        public TagArenaPlacement Placement;
    }

    public class ClassicArenaData : SharedArenaData
    {
        [JsonProperty("defenseHeroIds")]
        public int[] DefenseHeroIds;
    }

    public class WinLossRatio
    {
        [JsonProperty("total")]
        public int Total;

        [JsonProperty("wins")]
        public int Wins;

        [JsonProperty("losses")]
        public int Losses;
    }
}
