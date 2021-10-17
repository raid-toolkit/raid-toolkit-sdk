using Newtonsoft.Json;
using System.Linq;
using SharedModel.Meta.Skills;
using SharedModel.Battle.Effects;

namespace Raid.Service.DataModel
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
        public Visibility Visibility;

        [JsonProperty("unblockable")]
        public bool Unblockable;

        [JsonProperty("effects")]
        public EffectType[] Effects;

        [JsonProperty("upgrades")]
        public SkillUpgrade[] Upgrades;

        [JsonProperty("doesDamage")]
        public bool DoesDamage => Effects?.Any(effect => effect.KindId == EffectKindId.Damage) ?? false;
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
        public EffectKindId KindId;

        // TODO: there's a lot more data here we could extract
    }

    public class SkillUpgrade
    {
        [JsonProperty("type")]
        public SkillBonusType SkillBonusType;

        [JsonProperty("value")]
        public float Value;
    }

    public static partial class ModelExtensions
    {
        public static SkillType ToModel(this SharedModel.Meta.Skills.SkillType type)
        {
            return new SkillType()
            {
                TypeId = type.Id,
                Cooldown = type.Cooldown,
                Description = type.Description.ToModel(),
                Effects = type.Effects.Select(effect => effect.ToModel()).ToArray(),
                Name = type.Name.ToModel(),
                Unblockable = type.Unblockable.ToNullable(),
                Upgrades = type.SkillLevelBonuses?.Select(bonus => bonus.ToModel()).ToArray(),
                Visibility = type.Visibility,
            };
        }
        public static Skill ToModel(this SharedModel.Meta.Skills.Skill skill)
        {
            return new Skill()
            {
                Id = skill.Id,
                TypeId = skill.TypeId,
                Level = skill.Level,
            };
        }
        public static EffectType ToModel(this SharedModel.Battle.Effects.EffectType type)
        {
            return new EffectType()
            {
                TypeId = type.Id,
                KindId = type.KindId,
                Count = type.Count,
                StackCount = type.StackCount,
                Multiplier = type.MultiplierFormula,
            };
        }

        public static SkillUpgrade ToModel(this SharedModel.Meta.Skills.SkillBonus bonus)
        {
            return new SkillUpgrade()
            {
                SkillBonusType = bonus.SkillBonusType,
                Value = bonus.Value.AsFloat(),
            };
        }
    }
}