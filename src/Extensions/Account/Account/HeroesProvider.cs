using System;
using System.Collections.Generic;
using System.Linq;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility.Providers;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Il2CppToolkit.Runtime;
using SharedModel.Meta.Artifacts;

namespace Raid.Toolkit.Extension.Account
{
    public class HeroesProvider : DataProvider<AccountDataContext, HeroData>
    {
        private static Version kVersion = new(2, 0);

        public override string Key => "heroes";
        public override Version Version => kVersion;

        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        public HeroesProvider(CachedDataStorage<PersistedDataStorage> storage)
        {
            Storage = storage;
        }

        public override bool Upgrade(AccountDataContext context, Version dataVersion)
        {
            if (dataVersion == new Version(1, 0))
            {
                if (Storage.TryRead<IReadOnlyDictionary<int, Hero>>(context, Key, out var heroes))
                {
                    _ = Storage.Write(context, Key, new HeroData
                    {
                        Heroes = heroes,
                        BattlePresets = new Dictionary<int, int[]>()
                    });
                    return true;
                }
            }
            return false;
        }

        public override bool Update(Il2CsRuntimeContext runtime, AccountDataContext context)
        {
            if (!Storage.TryRead<StaticHeroTypeData>(StaticDataContext.Default, "heroTypes", out StaticHeroTypeData staticHeroTypes))
                return false;

            ModelScope scope = new(runtime);
            var userWrapper = scope.AppModel._userWrapper;
            var userHeroData = userWrapper.Heroes.HeroData;
            var heroTypes = staticHeroTypes.HeroTypes;

            var artifactsByHeroId = scope.AppModel._userWrapper.Artifacts.ArtifactData.ArtifactDataByHeroId;
            var heroesById = userHeroData.HeroById;

            // ignore result, and assume null below for missing value
            _ = Storage.TryRead(context, Key, out HeroData previous);

            // copy all previous deleted elements to save cost when looking later
            Dictionary<int, Hero> result = previous != null ? previous.Heroes.Filter(kvp => kvp.Value.Deleted) : new();
            foreach (var kvp in heroesById)
            {
                var id = kvp.Key;
                var hero = kvp.Value;
                if (hero == null) continue;

                var heroType = heroTypes[hero.TypeId];
                Dictionary<ArtifactKindId, int> equippedArtifacts = null;
                if (artifactsByHeroId.TryGetValue(id, out HeroArtifactData artifactData))
                {
                    equippedArtifacts = artifactData.ArtifactIdByKind.UnderlyingDictionary.Filter(kvp => kvp.Value != 0);
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

            return Storage.Write(context, Key, new HeroData
            {
                Heroes = result,
                BattlePresets = userHeroData.BattlePresets.UnderlyingDictionary
            });
        }
    }
}
