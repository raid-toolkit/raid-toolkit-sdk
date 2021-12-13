using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Raid.DataModel.Enums;

namespace Raid.DataModel
{
    public class StaticDataBase
    {
        [JsonProperty("gameHash")]
        public string Hash;
    }
    public class StaticData : StaticDataBase
    {
        [Obsolete("Use StaticHeroTypeData instead")]
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

    [Obsolete("Use StaticHeroTypeData instead")]
    public class StaticHeroData
    {
        [Obsolete("Use StaticHeroTypeData instead")]
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
}