using Newtonsoft.Json;
using SharedModel.Battle.Effects;
using SharedModel.Meta.Artifacts.Sets;

namespace Raid.Service.DataModel
{
    public class StatBonus
    {
        [JsonProperty("kind")]
        public StatKindId KindId;

        [JsonProperty("absolute")]
        public bool Absolute;

        [JsonProperty("value")]
        public float Value;
    }

    public static partial class ModelExtensions
    {
        public static StatBonus ToModel(this ArtifactSetStatBonus bonus)
        {
            return new()
            {
                Absolute = bonus.IsAbsolute,
                KindId = bonus.StatKindId,
                Value = bonus.Value.AsFloat(),
            };
        }
    }
}