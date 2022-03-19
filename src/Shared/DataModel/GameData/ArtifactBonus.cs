using Newtonsoft.Json;

namespace Raid.Toolkit.DataModel
{
    public class ArtifactStatBonus : StatBonus
    {
        [JsonProperty("glyphPower")]
        public double GlyphPower;

        [JsonProperty("level")]
        public int Level;
    }
}
