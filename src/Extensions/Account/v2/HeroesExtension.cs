using Client.Model.Gameplay.Artifacts;
using Il2CppToolkit.Runtime;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.DataModel.Enums;
using Raid.Toolkit.Extensibility;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extension.Account;

public class HeroesExtension :
    AccountDataExtensionBase,
    IAccountPublicApi<GetAccountDataApi<HeroData>>,
    GetAccountDataApi<HeroData>
{
    private const string Key = "heroes.json";

    GetAccountDataApi<HeroData> IAccountPublicApi<GetAccountDataApi<HeroData>>.GetApi() => this;
    bool GetAccountDataApi<HeroData>.TryGetData(out HeroData data) => Storage.TryRead(Key, out data);

    public HeroesExtension(IAccount account, IExtensionStorage storage, ILogger<HeroesExtension> logger)
    : base(account, storage, logger)
    {
    }

    protected override Task Update(ModelScope scope)
    {
        if (!Account.TryGetApi<GetAccountDataApi<StaticHeroTypeData>>(out var heroTypesApi)
            || !heroTypesApi.TryGetData(out StaticHeroTypeData staticHeroTypes))
            return Task.CompletedTask;


        var userWrapper = scope.AppModel._userWrapper;
        var userHeroData = userWrapper.Heroes.HeroData;
        var heroTypes = staticHeroTypes.HeroTypes;

        var artifactsByHeroId = scope.AppModel._userWrapper.Artifacts.ArtifactData.ArtifactDataByHeroId;
        var heroesById = userHeroData.HeroById;

        // ignore result, and assume null below for missing value
        _ = Storage.TryRead(Key, out HeroData previous);

        // copy all previous deleted elements to save cost when looking later
        Dictionary<int, Hero> result = previous != null ? previous.Heroes.Filter(kvp => kvp.Value.Deleted) : new();
        foreach (var kvp in heroesById)
        {
            var id = kvp.Key;
            var hero = kvp.Value;
            if (hero == null) continue;

            var heroType = heroTypes[hero.TypeId];
            Dictionary<ArtifactKindId, int> equippedArtifacts = new();
            if (artifactsByHeroId.TryGetValue(id, out SharedModel.Meta.Artifacts.HeroArtifactData artifactData))
            {
                equippedArtifacts = artifactData.ArtifactIdByKind.UnderlyingDictionary
                    .Filter(kvp => kvp.Value != 0)
                    .ToDictionary(kvp => (ArtifactKindId)kvp.Key, kvp => kvp.Value);
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

        Storage.Write(Key, new HeroData
        {
            Heroes = result,
            BattlePresets = userHeroData.BattlePresets.UnderlyingDictionary
        });
        return Task.CompletedTask;
    }
}
