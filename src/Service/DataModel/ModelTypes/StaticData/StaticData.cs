using System.Collections.Generic;
using Newtonsoft.Json;

namespace Raid.Service.DataModel
{
    public class StaticData
    {
        [JsonProperty("heroData")]
        public StaticHeroData HeroData;

        [JsonProperty("artifactData")]
        public StaticArtifactData ArtifactData;
    }
    public class StaticHeroData
    {
        [JsonProperty("heroTypes")]
        public IReadOnlyDictionary<int, HeroType> HeroTypes;
    }
    public class StaticArtifactData
    {
        [JsonProperty("setKinds")]
        public IReadOnlyDictionary<SharedModel.Meta.Artifacts.Sets.ArtifactSetKindId, ArtifactSetKind> ArtifactSetKinds;
    }
}