using System.Linq;
using Newtonsoft.Json;

namespace Raid.DataModel
{
    public class Skill
    {
        [JsonProperty("typeId")]
        public int TypeId;

        [JsonProperty("id")]
        public int Id;

        [JsonProperty("level")]
        public int Level;
    }

    public abstract class BaseSkillData
    {
        [JsonProperty("typeId")]
        public int TypeId;

        [JsonProperty("cooldown")]
        public int Cooldown;

        [JsonProperty("visibility")]
        public string Visibility;

        [JsonProperty("unblockable")]
        public bool Unblockable;

        [JsonProperty("effects")]
        public EffectType[] Effects;

        [JsonProperty("upgrades")]
        public SkillUpgrade[] Upgrades;

        [JsonProperty("doesDamage")]
        public bool DoesDamage => Effects?.Any(effect => effect.KindId == "Damage") ?? false;
        // TODO: there's a lot more data here we could extract
    }

    public class SkillType : BaseSkillData
    {
        [JsonProperty("name")]
        public LocalizedText Name;

        [JsonProperty("description")]
        public LocalizedText Description;
    }

    public class SkillSnapshot : BaseSkillData
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("level")]
        public int Level;

        public SkillSnapshot(SkillType skill)
        {
            Name = skill.Name.Localize();
            Description = skill.Description.Localize();
            TypeId = skill.TypeId;
            Cooldown = skill.Cooldown;
            Visibility = skill.Visibility;
            Unblockable = skill.Unblockable;
            Effects = skill.Effects;
            Upgrades = skill.Upgrades;
        }
    }

    public class EffectType
    {
        [JsonProperty("id")]
        public int TypeId;

        [JsonProperty("count")]
        public int Count;

        [JsonProperty("multiplier")]
        public string Multiplier;

        [JsonProperty("stack")]
        public int StackCount;

        [JsonProperty("kindId")]
        public string KindId;

        // TODO: there's a lot more data here we could extract
    }

    public class SkillUpgrade
    {
        [JsonProperty("type")]
        public string SkillBonusType;

        [JsonProperty("value")]
        public float Value;
    }
}