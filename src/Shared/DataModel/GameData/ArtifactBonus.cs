using Newtonsoft.Json;

namespace Raid.DataModel
{
    public class ArtifactStatBonus : StatBonus
    {
        [JsonProperty("glyphPower")]
        public double GlyphPower;

        [JsonProperty("level")]
        public int Level;
    }
}