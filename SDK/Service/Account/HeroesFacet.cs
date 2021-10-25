using System;
using System.Collections.Generic;
using System.Linq;
using Raid.Service.DataModel;
using SharedModel.Meta.Artifacts;
using SharedModel.Meta.Masteries;

namespace Raid.Service
{
    [Facet("heroes")]
    public class HeroesFacet : UserAccountFacetBase<IReadOnlyDictionary<int, Hero>, HeroesFacet>
    {
        private const int kForceRefreshInterval = 30000;
        private DateTime NextForcedRefresh = DateTime.MinValue;
        private int LastHeroId;

        private StaticDataCache StaticDataCache;
        public HeroesFacet(StaticDataCache staticDataCache) =>
            (StaticDataCache) = (staticDataCache);

        protected override IReadOnlyDictionary<int, Hero> Merge(ModelScope scope, IReadOnlyDictionary<int, Hero> previous = null)
        {
            var userWrapper = scope.AppModel._userWrapper;
            var userHeroData = userWrapper.Heroes.HeroData;
            var heroTypes = StaticDataFacet.ReadValue(StaticDataCache).HeroData.HeroTypes;

            // Only refresh if lastHeroId changed since last read, or after we've exceeded the forced read interval
            if (DateTime.UtcNow < NextForcedRefresh && userHeroData.LastHeroId == LastHeroId)
            {
                return previous;
            }
            NextForcedRefresh = DateTime.UtcNow.AddMilliseconds(kForceRefreshInterval);
            LastHeroId = userHeroData.LastHeroId;

            var artifactsByHeroId = scope.AppModel._userWrapper.Artifacts.ArtifactData.ArtifactDataByHeroId;
            var heroesById = userHeroData.HeroById;

            // copy all previous deleted elements to save cost when looking later
            Dictionary<int, Hero> result = previous != null ? new(previous.Where(kvp => kvp.Value.Deleted)) : new();
            foreach ((var id, var hero) in heroesById)
            {
                var heroType = heroTypes[hero.TypeId];
                Dictionary<ArtifactKindId, int> equippedArtifacts = null;
                if (artifactsByHeroId.TryGetValue(id, out HeroArtifactData artifactData))
                {
                    equippedArtifacts = artifactData.ArtifactIdByKind.UnderlyingDictionary;
                }

                IReadOnlyDictionary<int, int> skillLevels = hero.Skills.ToDictionary(skill => skill.TypeId, skill => skill.Level);

                Hero newHero = new()
                {
                    Id = id,
                    TypeId = hero.TypeId,
                    Level = hero.Level,
                    Marker = hero.Marker,
                    Rank = hero.Grade,
                    Locked = hero.Locked,
                    Deleted = false,
                    InVault = hero.InStorage,
                    Experience = hero.Experience,
                    FullExperience = hero.FullExperience,
                    Masteries = hero.MasteryData?.Masteries.Cast<MasteryKindId>().ToArray() ?? Array.Empty<MasteryKindId>(),
                    AssignedMasteryScrolls = hero.MasteryData?.TotalAmount.UnderlyingDictionary ?? new NumericDictionary<MasteryPointType, int>(),
                    UnassignedMasteryScrolls = hero.MasteryData?.CurrentAmount.UnderlyingDictionary ?? new NumericDictionary<MasteryPointType, int>(),
                    TotalMasteryScrolls = new NumericDictionary<MasteryPointType, int>(),
                    EquippedArtifactIds = equippedArtifacts,
                    Type = heroType,
                    Name = heroType.Name.Localize(),
#pragma warning disable 0618
                    SkillLevelsByTypeId = hero.Skills.ToDictionary(skill => skill.TypeId, skill => skill.Level),
#pragma warning restore 0618
                    SkillsById = hero.Skills.ToDictionary(skill => skill.Id, skill => skill.ToModel()),
                };

                newHero.TotalMasteryScrolls = new(newHero.TotalMasteryScrolls.Concat(newHero.AssignedMasteryScrolls)
                   .Concat(newHero.UnassignedMasteryScrolls)
                   .GroupBy(x => x.Key)
                   .ToDictionary(x => x.Key, x => x.Sum(y => y.Value)));

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
                            ascendedVersion.OriginalId = kvp.Key;
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