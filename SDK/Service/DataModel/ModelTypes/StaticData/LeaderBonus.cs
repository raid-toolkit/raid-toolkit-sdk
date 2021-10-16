using Newtonsoft.Json;
using SharedModel.Meta.Heroes;
using SharedModel.Meta.Skills;
using SharedModel.Meta.Stages;

namespace Raid.Service.DataModel
{
    public class LeaderStatBonus : StatBonus
    {
        [JsonProperty("area")]
        public AreaTypeId? AreaTypeId;

        [JsonProperty("affinity")]
        public Element? Affinity;
    }

    public static partial class ModelExtensions
    {
        public static LeaderStatBonus ToModel(this LeaderSkill skill)
        {
            return new()
            {
                Absolute = skill.IsAbsolute,
                Affinity = skill.Element.ToNullable(),
                AreaTypeId = skill.Area.ToNullable(),
                KindId = skill.StatKindId,
                Value = skill.Amount.AsFloat(),
            };
        }
    }
}