using Newtonsoft.Json;
using System.Linq;
using SharedModel.Meta.Heroes;
using SharedModel.Battle.AI.Utility;
using SharedModel.Meta.Skills;
using SharedModel.Battle.Effects;

namespace Raid.Service.DataModel
{
    public class SkillType
    {
        [JsonProperty("typeId")]
        public int TypeId;

        [JsonProperty("name")]
        public LocalizedText Name;

        [JsonProperty("cooldown")]
        public int Cooldown;

        [JsonProperty("description")]
        public LocalizedText Description;

        [JsonProperty("visibility")]
        public Visibility Visibility;

        [JsonProperty("unblockable")]
        public bool Unblockable;

        [JsonProperty("effects")]
        public EffectType[] Effects;// TODO

        [JsonProperty("upgrades")]
        public SkillUpgrade[] Upgrades;

        // TODO: there's a lot more data here we could extract
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

        [JsonProperty("stack")]
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
                Upgrades = type.SkillLevelBonuses.Select(bonus => bonus.ToModel()).ToArray(),
                Visibility = type.Visibility,
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