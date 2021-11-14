using System;
using System.Collections.Generic;
using System.Linq;
using Raid.DataModel;
using SharedModel.Meta.Artifacts;
using SharedModel.Meta.Masteries;

namespace Raid.Service
{
    [Facet("heroes")]
    public class HeroesFacet : UserAccountFacetBase<IReadOnlyDictionary<int, Hero>, HeroesFacet>
    {
        private StaticDataCache StaticDataCache;
        public HeroesFacet(StaticDataCache staticDataCache) =>
            (StaticDataCache) = (staticDataCache);

        protected override IReadOnlyDictionary<int, Hero> Merge(ModelScope scope, IReadOnlyDictionary<int, Hero> previous = null)
        {
            var userWrapper = scope.AppModel._userWrapper;
            var userHeroData = userWrapper.Heroes.HeroData;
            var heroTypes = StaticDataFacet.ReadValue(StaticDataCache).HeroData.HeroTypes;

            var artifactsByHeroId = scope.AppModel._userWrapper.Artifacts.ArtifactData.ArtifactDataByHeroId;
            var heroesById = userHeroData.HeroById;

            // copy all previous deleted elements to save cost when looking later
            Dictionary<int, Hero> result = previous != null ? new(previous.Where(kvp => kvp.Value.Deleted)) : new();
            foreach ((var id, var hero) in heroesById)
            {
                if (hero == null) continue;

                var heroType = heroTypes[hero.TypeId];
                Dictionary<ArtifactKindId, int> equippedArtifacts = null;
                if (artifactsByHeroId.TryGetValue(id, out HeroArtifactData artifactData))
                {
                    equippedArtifacts = artifactData.ArtifactIdByKind.UnderlyingDictionary;
                }

                IReadOnlyDictionary<int, int> skillLevels = hero.Skills.ToDictionary(skill => skill.TypeId, skill => skill.Level);

                Hero newHero = hero.ToModel(equippedArtifacts, heroType);

                result.Add(id, newHero);
            }

            if (previous != null)
            {
                foreach (var kvp in previous)
                {
                    // deleted hero?
                    if (!result.ContainsKey(kvp.Key))
                    {
                        // find any hero which was added at a higher ascension level
                        var ascendedVersion = result.Values.FirstOrDefault(hero => hero.TypeId == (kvp.Value.TypeId + 1) && !previous.ContainsKey(hero.Id));
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

            return result;
        }
    }
}