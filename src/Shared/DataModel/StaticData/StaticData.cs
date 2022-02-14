using System.Collections.Generic;
using Newtonsoft.Json;
using Raid.Toolkit.DataModel.Enums;

namespace Raid.Toolkit.DataModel
{
    public class StaticDataBase
    {
        [JsonProperty("gameHash")]
        public string Hash;
    }
    public class StaticData : StaticDataBase
    {
        [JsonProperty("heroData")]
        public StaticHeroTypeData HeroData;

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

    public class StaticArtifactData : StaticDataBase
    {
        [JsonProperty("setKinds")]
        public IReadOnlyDictionary<string, ArtifactSetKind> ArtifactSetKinds;
    }

    public class StaticSkillData : StaticDataBase
    {
        [JsonProperty("skillTypes")]
        public IReadOnlyDictionary<int, SkillType> SkillTypes;
    }

    public class StaticArenaData : StaticDataBase
    {
        [JsonProperty("leagues")]
        public IReadOnlyDictionary<string, ArenaLeague> Leagues;
    }

    public class StaticStageData : StaticDataBase
    {
        [JsonProperty("areas")]
        public IReadOnlyDictionary<string, AreaData> Areas;

        [JsonProperty("regions")]
        public IReadOnlyDictionary<string, RegionData> Regions;

        [JsonProperty("stages")]
        public IReadOnlyDictionary<int, StageData> Stages;
    }

    public class StaticAcademyData : StaticDataBase
    {
        [JsonProperty("guardianBonusByRarity")]
        public IReadOnlyDictionary<HeroRarity, StatBonus[][]> GuardianBonusByRarity;
    }

    public class StaticHeroTypeData : StaticDataBase
    {
        [JsonProperty("heroTypes")]
        public IReadOnlyDictionary<int, HeroType> HeroTypes;
    }

    public class StaticLocalizationData : StaticDataBase
    {
        [JsonProperty("localizedStrings")]
        public IReadOnlyDictionary<string, string> LocalizedStrings;
    }
}
