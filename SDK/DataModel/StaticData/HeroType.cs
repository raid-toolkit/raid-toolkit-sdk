using Newtonsoft.Json;
using System.Linq;

namespace Raid.DataModel
{
    public class HeroType
    {
        [JsonProperty("typeId")]
        public int TypeId;

        [JsonProperty("name")]
        public LocalizedText Name;

        [JsonProperty("affinity")]
        public string Affinity;

        [JsonProperty("faction")]
        public string Faction;

        [JsonProperty("role")]
        public string Role;

        [JsonProperty("rarity")]
        public string Rarity;

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