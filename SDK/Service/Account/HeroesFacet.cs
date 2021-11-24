using System;
using System.Collections.Generic;
using System.Linq;
using Raid.DataModel;
using SharedModel.Meta.Artifacts;

namespace Raid.Service
{
    [Facet("heroes", Version = "2.1")]
    public class HeroesFacet : UserAccountFacetBase<HeroData, HeroesFacet>
    {
        private readonly StaticDataCache StaticDataCache;
        public HeroesFacet(StaticDataCache staticDataCache)
        {
            StaticDataCache = staticDataCache;
        }

        public override bool TryUpgrade(IModelDataSource dataSource, Version from, out HeroData upgradedData)
        {
            if (from == new Version(1, 0))
            {
                var heroes = dataSource.Read<IReadOnlyDictionary<int, Hero>>("heroes");
                if (heroes != null)
                {
                    upgradedData = new HeroData()
                    {
                        Heroes = heroes,
                        BattlePresets = new Dictionary<int, int[]>()
                    };
                    return true;
                }
            }
            upgradedData = null;
            return false;
        }

        protected override HeroData Merge(ModelScope scope, HeroData previous = null)
        {
            var userWrapper = scope.AppModel._userWrapper;
            var userHeroData = userWrapper.Heroes.HeroData;
            var heroTypes = StaticDataFacet.ReadValue(StaticDataCache).HeroData.HeroTypes;

            var artifactsByHeroId = scope.AppModel._userWrapper.Artifacts.ArtifactData.ArtifactDataByHeroId;
            var heroesById = userHeroData.HeroById;

            // copy all previous deleted elements to save cost when looking later
            Dictionary<int, Hero> result = previous != null ? new(previous.Heroes.Where(kvp => kvp.Value.Deleted)) : new();
            foreach ((var id, var hero) in heroesById)
            {
                if (hero == null) continue;

                var heroType = heroTypes[hero.TypeId];
                Dictionary<ArtifactKindId, int> equippedArtifacts = null;
                if (artifactsByHeroId.TryGetValue(id, out HeroArtifactData artifactData))
                {
                    equippedArtifacts = new(artifactData.ArtifactIdByKind.UnderlyingDictionary.Where(kvp => kvp.Value != 0));
                }

                IReadOnlyDictionary<int, int> skillLevels = hero.Skills.ToDictionary(skill => skill.TypeId, skill => skill.Level);

                Hero newHero = hero.ToModel(equippedArtifacts, heroType);

                result.Add(id, newHero);
            }

            if (previous != null)
            {
                foreach (var kvp in previous.Heroes)
                {
                    // deleted hero?
                    if (!result.ContainsKey(kvp.Key))
                    {
                        // find any hero which was added at a higher ascension level
                        var ascendedVersion = result.Values.FirstOrDefault(hero => hero.TypeId == (kvp.Value.TypeId + 1) && !previous.Heroes.ContainsKey(hero.Id));
                        if (ascendedVersion != null)
                        {
                            if (ascendedVersion.OriginalId == 0)
                            {
                                ascendedVersion.OriginalId = kvp.Key;
                            }
                        }
                        else
                        {
                            kvp.Value.Deleted = true;
                            result.Add(kvp.Key, kvp.Value);
                        }
                    }
                }
            }

            return new HeroData()
            {
                Heroes = result,
                BattlePresets = userHeroData.BattlePresets.UnderlyingDictionary
            };
        }
    }
}
