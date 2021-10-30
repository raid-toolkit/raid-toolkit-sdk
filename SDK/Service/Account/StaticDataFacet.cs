using System.Collections.Generic;
using System.Linq;
using Raid.DataModel;
using SharedModel.Meta.Stages;

namespace Raid.Service
{
    [Facet("staticData")]
    public class StaticDataFacet : StaticFacetBase<StaticData, StaticDataFacet>
    {
        protected override StaticData Merge(ModelScope scope, StaticData previous = null)
        {
            var hash = scope.StaticDataManager._hash;
            if (previous?.Hash == hash)
            {
                return previous;
            }
            var staticData = scope.StaticDataManager.StaticData;
            var heroTypes = staticData.HeroData.HeroTypeById.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel());
            var artifactTypes = staticData.ArtifactData._setInfoByKind.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel());
            var skillTypes = staticData.SkillData._skillTypeById.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel());
            var arenaLeagues = staticData.ArenaData.LeagueInfoById.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel());
            var localizedStrings = new Dictionary<string, string>(staticData.ClientLocalization.Concat(staticData.StaticDataLocalization));

            var areas = new Dictionary<AreaTypeId, AreaData>();
            var regions = new Dictionary<RegionTypeId, RegionData>();
            var stages = new List<KeyValuePair<int, StageData>>();
            foreach (var area in staticData.StageData.Areas)
            {
                areas.Add(area.Id, area.ToModel());
                foreach (var region in area.Regions)
                {
                    regions.Add(region.Id, region.ToModel(area.Id));
                    foreach (var stagesList in region.StagesByDifficulty.Values)
                    {
                        stages.AddRange(
                            stagesList.ToDictionary(stage => stage.Id, stage => stage.ToModel(area.Id, region.Id)).AsEnumerable()
                            );
                    }
                }
            }

            return new()
            {
                Hash = hash,
                HeroData = new() { HeroTypes = heroTypes, },
                ArtifactData = new() { ArtifactSetKinds = artifactTypes.ToModel() },
                SkillData = new() { SkillTypes = skillTypes },
                ArenaData = new() { Leagues = arenaLeagues.ToModel() },
                StageData = new()
                {
                    Areas = areas.ToModel(),
                    Regions = regions.ToModel(),
                    Stages = new Dictionary<int, StageData>(stages)
                },
                LocalizedStrings = localizedStrings
            };
        }
    }
}
