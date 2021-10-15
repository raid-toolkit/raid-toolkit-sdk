using System;
using System.Collections.Generic;
using System.Linq;
using Raid.Service.DataModel;

namespace Raid.Service
{
    [Facet("staticData")]
    public class StaticDataFacet : StaticFacetBase<StaticData, StaticDataFacet>
    {
        protected override StaticData Merge(ModelScope scope, StaticData previous = null)
        {
            var staticData = scope.StaticDataManager.StaticData;
            var heroTypes = staticData.HeroData.HeroTypeById.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel());
            var artifactTypes = staticData.ArtifactData._setInfoByKind.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel());

            return new()
            {
                HeroData = new() { HeroTypes = heroTypes, },
                ArtifactData = new() { ArtifactSetKinds = artifactTypes }
            };
        }
    }
}
