using Microsoft.Extensions.Logging;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.DataModel.Enums;
using Raid.Toolkit.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extension.Account;

public class StaticTypesExtension :
    AccountDataExtensionBase,
    IAccountPublicApi<GetAccountDataApi<StaticArtifactData>>,
    GetAccountDataApi<StaticArtifactData>,
    IAccountPublicApi<GetAccountDataApi<StaticHeroTypeData>>,
    GetAccountDataApi<StaticHeroTypeData>,
    IAccountPublicApi<GetAccountDataApi<StaticSkillData>>,
    GetAccountDataApi<StaticSkillData>,
    IAccountPublicApi<GetAccountDataApi<StaticLocalizationData>>,
    GetAccountDataApi<StaticLocalizationData>,
    IAccountPublicApi<GetAccountDataApi<StaticArenaData>>,
    GetAccountDataApi<StaticArenaData>,
    IAccountPublicApi<GetAccountDataApi<StaticAcademyData>>,
    GetAccountDataApi<StaticAcademyData>,
    IAccountPublicApi<GetAccountDataApi<StaticStageData>>,
    GetAccountDataApi<StaticStageData>
{
    private const string ArtifactTypesKey = "artifact-types.static.json";
    private const string HeroTypesKey = "hero-types.static.json";
    private const string SkillTypesKey = "skill-types.static.json";
    private const string ArenaKey = "arena.static.json";
    private const string AcademyKey = "academy.static.json";
    private const string StagesKey = "stages.static.json";
    private const string StringsKey = "strings.static.json";
    private static string? StaticDataHash;

    GetAccountDataApi<StaticArtifactData> IAccountPublicApi<GetAccountDataApi<StaticArtifactData>>.GetApi() => this;
    bool GetAccountDataApi<StaticArtifactData>.TryGetData(out StaticArtifactData data)
        => Storage.TryRead(ArtifactTypesKey, out data);

    GetAccountDataApi<StaticHeroTypeData> IAccountPublicApi<GetAccountDataApi<StaticHeroTypeData>>.GetApi() => this;
    bool GetAccountDataApi<StaticHeroTypeData>.TryGetData(out StaticHeroTypeData data)
        => Storage.TryRead(HeroTypesKey, out data);

    GetAccountDataApi<StaticSkillData> IAccountPublicApi<GetAccountDataApi<StaticSkillData>>.GetApi() => this;
    bool GetAccountDataApi<StaticSkillData>.TryGetData(out StaticSkillData data)
        => Storage.TryRead(SkillTypesKey, out data);

    GetAccountDataApi<StaticLocalizationData> IAccountPublicApi<GetAccountDataApi<StaticLocalizationData>>.GetApi() => this;
    bool GetAccountDataApi<StaticLocalizationData>.TryGetData(out StaticLocalizationData data)
        => Storage.TryRead(StringsKey, out data);

    GetAccountDataApi<StaticArenaData> IAccountPublicApi<GetAccountDataApi<StaticArenaData>>.GetApi() => this;
    bool GetAccountDataApi<StaticArenaData>.TryGetData(out StaticArenaData data)
        => Storage.TryRead(ArenaKey, out data);

    GetAccountDataApi<StaticAcademyData> IAccountPublicApi<GetAccountDataApi<StaticAcademyData>>.GetApi() => this;
    bool GetAccountDataApi<StaticAcademyData>.TryGetData(out StaticAcademyData data)
        => Storage.TryRead(AcademyKey, out data);

    GetAccountDataApi<StaticStageData> IAccountPublicApi<GetAccountDataApi<StaticStageData>>.GetApi() => this;
    bool GetAccountDataApi<StaticStageData>.TryGetData(out StaticStageData data)
        => Storage.TryRead(StagesKey, out data);

    public StaticTypesExtension(IAccount account, IExtensionStorage storage, ILogger<HeroesExtension> logger)
    : base(account, storage, logger)
    {
    }

    private T? EnsureTypesRead<T>(string key, Func<T> readFn) where T : StaticDataBase
    {
        if (StaticDataHash == null)
            return null;

        try
        {
            if (Storage.TryRead(key, out T data) && data?.Hash == StaticDataHash)
            {
                return data;
            }
            data = readFn();
            data.Hash = StaticDataHash;
            Storage.Write(key, data);
            return data;
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"Failed to update static data {key}");
            return null;
        }
    }

    protected override Task Update(ModelScope scope)
    {
        StaticDataHash ??= scope.StaticDataManager._hash;

        EnsureTypesRead(ArtifactTypesKey, () =>
        {
            var artifactTypes = scope.StaticDataManager.StaticData.ArtifactData._setInfoByKind.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel());
            return new StaticArtifactData { ArtifactSetKinds = artifactTypes.ToModel() };
        });

        EnsureTypesRead(HeroTypesKey, () =>
        {
            var heroTypes = scope.StaticDataManager.StaticData.HeroData.HeroTypeById.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel());
            return new StaticHeroTypeData() { HeroTypes = heroTypes };
        });

        EnsureTypesRead(SkillTypesKey, () =>
        {
            var skillTypes = scope.StaticDataManager.StaticData.SkillData._skillTypeById.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel());
            return new StaticSkillData() { SkillTypes = skillTypes };
        });

        EnsureTypesRead(StringsKey, () =>
        {
            var staticData = scope.StaticDataManager.StaticData;
            Dictionary<string, string> localizedStrings = new();
            foreach (var entry in staticData.ClientLocalization.Concat(staticData.StaticDataLocalization)
                .GroupBy(x => x.Key)
                .Select(g => g.First()))
            {
                localizedStrings.Add(entry.Key, entry.Value);
            }
            return new StaticLocalizationData() { LocalizedStrings = localizedStrings };
        });

        EnsureTypesRead(ArenaKey, () =>
        {
            var arenaLeagues = scope.StaticDataManager.StaticData.ArenaData.LeagueInfoById.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel()).ToModel();
            return new StaticArenaData() { Leagues = arenaLeagues };
        });

        EnsureTypesRead(AcademyKey, () =>
        {
            var guardiansBonusData = scope.StaticDataManager.StaticData.AcademyData.Guardians.BonusesByHeroRarity.ToDictionary(
                kvp => (HeroRarity)kvp.Key,
                kvp => kvp.Value.Select(bonuses => bonuses.Bonuses.Select(bonus => bonus.Value.ToModel(bonus.Key)).ToArray()).ToArray()
            );
            return new StaticAcademyData() { GuardianBonusByRarity = guardiansBonusData };
        });

        EnsureTypesRead(StagesKey, () =>
        {
            var staticData = scope.StaticDataManager.StaticData;
            var areas = new Dictionary<SharedModel.Meta.Stages.AreaTypeId, AreaData>();
            var regions = new Dictionary<SharedModel.Meta.Stages.RegionTypeId, RegionData>();
            Dictionary<int, StageData> stages = new();
            foreach (var area in staticData.StageData.Areas)
            {
                areas.Add(area.Id, area.ToModel());
                foreach (var region in area.Regions)
                {
                    regions.Add(region.Id, region.ToModel(area.Id));
                    foreach (var stagesList in region.StagesByDifficulty.Values)
                    {
                        foreach (var entry in stagesList.ToDictionary(stage => stage.Id, stage => stage.ToModel(area.Id, region.Id)))
                        {
                            stages.Add(entry.Key, entry.Value);
                        }
                    }
                }
            }
            return new StaticStageData()
            {
                Areas = areas.ToModel(),
                Regions = regions.ToModel(),
                Stages = stages
            };
        });

        return Task.CompletedTask;
    }
}
