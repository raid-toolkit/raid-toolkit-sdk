using System.Collections.Generic;
using Newtonsoft.Json;

namespace Raid.DataModel
{
    public class StaticData
    {
        [JsonProperty("gameHash")]
        public string Hash;

        [JsonProperty("heroData")]
        public StaticHeroData HeroData;

        [JsonProperty("artifactData")]
        public StaticArtifactData ArtifactData;

        [JsonProperty("skillData")]
        public StaticSkillData SkillData;

        [JsonProperty("arenaData")]
        public StaticArenaData ArenaData;

        [JsonProperty("stageData")]
        public StaticStageData StageData;

        [JsonProperty("localizedStrings")]
        public IReadOnlyDictionary<string, string> LocalizedStrings;
    }

    public class StaticHeroData
    {
        [JsonProperty("heroTypes")]
        public IReadOnlyDictionary<int, HeroType> HeroTypes;
    }

    public class StaticArtifactData
    {
        [JsonProperty("setKinds")]
        public IReadOnlyDictionary<string, ArtifactSetKind> ArtifactSetKinds;
    }

    public class StaticSkillData
    {
        [JsonProperty("skillTypes")]
        public IReadOnlyDictionary<int, SkillType> SkillTypes;
    }

    public class StaticArenaData
    {
        [JsonProperty("leagues")]
        public IReadOnlyDictionary<string, ArenaLeague> Leagues;
    }

    public class StaticStageData
    {
        [JsonProperty("areas")]
        public IReadOnlyDictionary<string, AreaData> Areas;

        [JsonProperty("regions")]
        public IReadOnlyDictionary<string, RegionData> Regions;

        [JsonProperty("stages")]
        public IReadOnlyDictionary<int, StageData> Stages;
    }
}