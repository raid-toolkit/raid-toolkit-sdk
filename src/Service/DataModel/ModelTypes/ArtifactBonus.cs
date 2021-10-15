using Newtonsoft.Json;

namespace Raid.Service.DataModel
{
    public class ArtifactStatBonus : StatBonus
    {
        [JsonProperty("glyphPower")]
        public double GlyphPower;

        [JsonProperty("level")]
        public int Level;
    }
    public static partial class ModelExtensions
    {
        public static ArtifactStatBonus ToModel(this SharedModel.Meta.Artifacts.Bonuses.ArtifactBonus bonus)
        {
            return new()
            {
                KindId = bonus._kindId,
                Absolute = bonus._value._isAbsolute,
                Value = bonus._value._value.AsFloat(),
                GlyphPower = bonus._powerUpValue.AsFloat(),
                Level = bonus._level
            };
        }
    }
}