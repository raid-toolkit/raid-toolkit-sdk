using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Raid.Toolkit.DataModel.Enums;

using System;

namespace Raid.Toolkit.DataModel
{
    public class HeroType
    {
        [JsonProperty("typeId")]
        public int TypeId = - 1;

        [JsonProperty("name")]
        public LocalizedText? Name;

        [JsonProperty("shortName")]
        public LocalizedText? ShortName;

        [JsonProperty("affinity"), JsonConverter(typeof(StringEnumConverter))]
        public Element? Affinity;

        [JsonProperty("faction"), JsonConverter(typeof(StringEnumConverter))]
        public HeroFraction? Faction;

        [JsonProperty("roles"), JsonConverter(typeof(StringEnumConverter))]
        public HeroRole[]? Roles;

        [JsonProperty("rarity"), JsonConverter(typeof(StringEnumConverter))]
        public HeroRarity? Rarity;

        [JsonProperty("ascended")]
        public int Ascended = 0;

        [JsonProperty("modelName")]
        public string? ModelName;

        [JsonProperty("avatarKey")]
        public string? AvatarKey;

        [JsonProperty("leaderSkill")]
        public LeaderStatBonus? LeaderSkill;

        [JsonProperty("skillTypeIds")]
        public int[]? SkillTypeIds;

        [JsonProperty("unscaledStats")]
        public Stats? UnscaledStats;

        [Obsolete("No longer used"), JsonProperty("brain")]
        public string? Brain;
    }
}
