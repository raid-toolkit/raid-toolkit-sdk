using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Raid.Toolkit.DataModel.Enums;

namespace Raid.Toolkit.DataModel
{
    public class HeroType
    {
        [JsonProperty("typeId")]
        public int TypeId;

        [JsonProperty("name")]
        public LocalizedText Name;

        [JsonProperty("affinity"), JsonConverter(typeof(StringEnumConverter))]
        public Element Affinity;

        [JsonProperty("faction"), JsonConverter(typeof(StringEnumConverter))]
        public HeroFraction Faction;

        [JsonProperty("role"), JsonConverter(typeof(StringEnumConverter))]
        public HeroRole Role;

        [JsonProperty("rarity"), JsonConverter(typeof(StringEnumConverter))]
        public HeroRarity Rarity;

        [JsonProperty("ascended")]
        public int Ascended;

        [JsonProperty("modelName")]
        public string ModelName;

        [JsonProperty("avatarKey")]
        public string AvatarKey;

        [JsonProperty("leaderSkill")]
        public LeaderStatBonus LeaderSkill;

        [JsonProperty("skillTypeIds")]
        public int[] SkillTypeIds;

        [JsonProperty("unscaledStats")]
        public Stats UnscaledStats;

        [JsonProperty("brain")]
        public string Brain;
    }
}
