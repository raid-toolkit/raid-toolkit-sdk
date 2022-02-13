using System;
using System.Collections.Generic;
using System.Linq;
using Raid.DataModel;
using Raid.DataServices;
using SharedModel.Meta.Artifacts;

namespace Raid.Service.DataServices
{
    [DataType("heroes")]
    public class HeroDataObject : HeroData
    {
    }

    public class HeroesProvider : DataProviderBase<AccountDataContext, HeroDataObject>
    {
        private readonly StaticHeroTypeProvider HeroTypes;
        public HeroesProvider(
            IDataResolver<AccountDataContext, CachedDataStorage<PersistedDataStorage>, HeroDataObject> storage,
            StaticHeroTypeProvider heroTypes)
            : base(storage)
        {
            HeroTypes = heroTypes;
        }

        public override bool Upgrade(AccountDataContext context, Version dataVersion)
        {
            if (dataVersion == new Version(1, 0))
            {
                if (PrimaryProvider.TryReadAs<IReadOnlyDictionary<int, Hero>>(context, out var heroes))
                {
                    _ = PrimaryProvider.Write(context, new HeroDataObject()
                    {
                        Heroes = heroes,
                        BattlePresets = new Dictionary<int, int[]>()
                    });
                    return true;
                }
            }
            return false;
        }

        public override bool Update(ModelScope scope, AccountDataContext context)
        {
            var userWrapper = scope.AppModel._userWrapper;
            var userHeroData = userWrapper.Heroes.HeroData;
            var heroTypes = HeroTypes.GetValue(StaticDataContext.Default).HeroTypes;

            var artifactsByHeroId = scope.AppModel._userWrapper.Artifacts.ArtifactData.ArtifactDataByHeroId;
            var heroesById = userHeroData.HeroById;

            // ignore result, and assume null below for missing value
            _ = PrimaryProvider.TryRead(context, out HeroDataObject previous);

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

            return PrimaryProvider.Write(context, new HeroDataObject()
            {
                Heroes = result,
                BattlePresets = userHeroData.BattlePresets.UnderlyingDictionary
            });
        }
    }
}
