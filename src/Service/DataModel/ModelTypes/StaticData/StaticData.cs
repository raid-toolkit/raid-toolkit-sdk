using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using SharedModel.Meta.Battle.Arena;
using SharedModel.Meta.Stages;

namespace Raid.Service.DataModel
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IReadOnlyDictionary<int, ArtifactSetKind> _ArtifactSetKinds
        {
            get { return ArtifactSetKinds?.ToDictionary(kvp => (int)kvp.Key, kvp => kvp.Value); }
            set { ArtifactSetKinds = value?.ToDictionary(kvp => (SharedModel.Meta.Artifacts.Sets.ArtifactSetKindId)kvp.Key, kvp => kvp.Value); }
        }

        [JsonIgnore]
        public IReadOnlyDictionary<SharedModel.Meta.Artifacts.Sets.ArtifactSetKindId, ArtifactSetKind> ArtifactSetKinds;
    }

    public class StaticSkillData
    {
        [JsonProperty("skillTypes")]
        public IReadOnlyDictionary<int, SkillType> SkillTypes;
    }

    public class StaticArenaData
    {
        [JsonProperty("leagues")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IReadOnlyDictionary<int, ArenaLeague> _Leagues
        {
            get { return Leagues?.ToDictionary(kvp => (int)kvp.Key, kvp => kvp.Value); }
            set { Leagues = value?.ToDictionary(kvp => (ArenaLeagueId)kvp.Key, kvp => kvp.Value); }
        }

        [JsonIgnore]
        public IReadOnlyDictionary<ArenaLeagueId, ArenaLeague> Leagues;
    }

    public class StaticStageData
    {
        [JsonProperty("areas")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IReadOnlyDictionary<int, AreaData> _Areas
        {
            get { return Areas?.ToDictionary(kvp => (int)kvp.Key, kvp => kvp.Value); }
            set { Areas = value?.ToDictionary(kvp => (AreaTypeId)kvp.Key, kvp => kvp.Value); }
        }

        [JsonIgnore]
        public IReadOnlyDictionary<AreaTypeId, AreaData> Areas;

        [JsonProperty("regions")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IReadOnlyDictionary<int, RegionData> _Regions
        {
            get { return Regions?.ToDictionary(kvp => (int)kvp.Key, kvp => kvp.Value); }
            set { Regions = value?.ToDictionary(kvp => (RegionTypeId)kvp.Key, kvp => kvp.Value); }
        }

        [JsonIgnore]
        public IReadOnlyDictionary<RegionTypeId, RegionData> Regions;

        [JsonProperty("stages")]
        public IReadOnlyDictionary<int, StageData> Stages;
    }
}