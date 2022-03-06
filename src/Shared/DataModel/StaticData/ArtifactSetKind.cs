using Newtonsoft.Json;

namespace Raid.Toolkit.DataModel
{
    public class ArtifactSetKind
    {
        [JsonProperty("setKindId")]
        public string SetKindId;

        [JsonProperty("artifactCount")]
        public int ArtifactCount;

        [JsonProperty("name")]
        public LocalizedText Name;

        [JsonProperty("statBonuses")]
        public StatBonus[] StatBonuses;

        [JsonProperty("skillBonus")]
        public int? SkillBonus;

        [JsonProperty("shortDescription")]
        public LocalizedText ShortDescription;

        [JsonProperty("longDescription")]
        public LocalizedText LongDescription;
    }
}
