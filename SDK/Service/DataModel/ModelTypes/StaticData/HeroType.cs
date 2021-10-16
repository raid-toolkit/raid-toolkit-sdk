using Newtonsoft.Json;
using SharedModel.Meta.Heroes;
using SharedModel.Battle.AI.Utility;
using System.Linq;

namespace Raid.Service.DataModel
{
    public class HeroType
    {
        [JsonProperty("typeId")]
        public int TypeId;

        [JsonProperty("name")]
        public LocalizedText Name;

        [JsonProperty("affinity")]
        public Element Affinity;

        [JsonProperty("faction")]
        public HeroFraction Faction;

        [JsonProperty("role")]
        public HeroRole Role;

        [JsonProperty("rarity")]
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
        public Brain Brain;
    }

    public static partial class ModelExtensions
    {
        public static HeroType ToModel(this SharedModel.Meta.Heroes.HeroType type)
        {
            return new HeroType()
            {
                Affinity = type.Element,
                Ascended = type.Id % 10,
                AvatarKey = type.AvatarName,
                Faction = type.Fraction,
                ModelName = type.ModelName,
                Name = type.Name.ToModel(),
                Rarity = type.Rarity,
                Role = type.Role,
                LeaderSkill = type.LeaderSkill?.ToModel(),
                SkillTypeIds = type.SkillTypeIds?.ToArray(),
                TypeId = type.Id,
                UnscaledStats = type.BaseStats.ToModel(),
                Brain = type.Brain,
            };
        }
    }
}