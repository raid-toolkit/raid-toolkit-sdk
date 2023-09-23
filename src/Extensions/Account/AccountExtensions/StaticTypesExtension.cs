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
    IAccountPublicApi<IGetAccountDataApi<StaticArtifactData>>,
    IGetAccountDataApi<StaticArtifactData>,
    IAccountPublicApi<IGetAccountDataApi<StaticHeroTypeData>>,
    IGetAccountDataApi<StaticHeroTypeData>,
    IAccountPublicApi<IGetAccountDataApi<StaticSkillData>>,
    IGetAccountDataApi<StaticSkillData>,
    IAccountPublicApi<IGetAccountDataApi<StaticLocalizationData>>,
    IGetAccountDataApi<StaticLocalizationData>,
    IAccountPublicApi<IGetAccountDataApi<StaticArenaData>>,
    IGetAccountDataApi<StaticArenaData>,
    IAccountPublicApi<IGetAccountDataApi<StaticAcademyData>>,
    IGetAccountDataApi<StaticAcademyData>,
    IAccountPublicApi<IGetAccountDataApi<StaticStageData>>,
    IGetAccountDataApi<StaticStageData>
{
    private const string ArtifactTypesKey = "artifact-types.static.json";
    private const string HeroTypesKey = "hero-types.static.json";
    private const string SkillTypesKey = "skill-types.static.json";
    private const string ArenaKey = "arena.static.json";
    private const string AcademyKey = "academy.static.json";
    private const string StagesKey = "stages.static.json";
    private const string StringsKey = "strings.static.json";
    private static string? StaticDataHash;

    IGetAccountDataApi<StaticArtifactData> IAccountPublicApi<IGetAccountDataApi<StaticArtifactData>>.GetApi() => this;
    bool IGetAccountDataApi<StaticArtifactData>.TryGetData(out StaticArtifactData data)
        => Storage.TryRead(ArtifactTypesKey, out data);

    IGetAccountDataApi<StaticHeroTypeData> IAccountPublicApi<IGetAccountDataApi<StaticHeroTypeData>>.GetApi() => this;
    bool IGetAccountDataApi<StaticHeroTypeData>.TryGetData(out StaticHeroTypeData data)
        => Storage.TryRead(HeroTypesKey, out data);

    IGetAccountDataApi<StaticSkillData> IAccountPublicApi<IGetAccountDataApi<StaticSkillData>>.GetApi() => this;
    bool IGetAccountDataApi<StaticSkillData>.TryGetData(out StaticSkillData data)
        => Storage.TryRead(SkillTypesKey, out data);

    IGetAccountDataApi<StaticLocalizationData> IAccountPublicApi<IGetAccountDataApi<StaticLocalizationData>>.GetApi() => this;
    bool IGetAccountDataApi<StaticLocalizationData>.TryGetData(out StaticLocalizationData data)
        => Storage.TryRead(StringsKey, out data);

    IGetAccountDataApi<StaticArenaData> IAccountPublicApi<IGetAccountDataApi<StaticArenaData>>.GetApi() => this;
    bool IGetAccountDataApi<StaticArenaData>.TryGetData(out StaticArenaData data)
        => Storage.TryRead(ArenaKey, out data);

    IGetAccountDataApi<StaticAcademyData> IAccountPublicApi<IGetAccountDataApi<StaticAcademyData>>.GetApi() => this;
    bool IGetAccountDataApi<StaticAcademyData>.TryGetData(out StaticAcademyData data)
        => Storage.TryRead(AcademyKey, out data);

    IGetAccountDataApi<StaticStageData> IAccountPublicApi<IGetAccountDataApi<StaticStageData>>.GetApi() => this;
    bool IGetAccountDataApi<StaticStageData>.TryGetData(out StaticStageData data)
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
            Logger.LogInformation("Reading static data for {key}", key);
            if (Storage.TryRead(key, out T data) && data?.Hash == StaticDataHash)
            {
                return data;
            }
            data = readFn();
            data.Hash = StaticDataHash;
            Storage.Write(key, data);
            Logger.LogInformation("Completed static data for {key}", key);
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

        var localizationData = EnsureTypesRead(StringsKey, () =>
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

        if (localizationData != null)
        {
            StaticResources.AddStrings(localizationData.LocalizedStrings);
        }

        EnsureTypesRead(HeroTypesKey, () =>
        {
            var heroTypes = scope.StaticDataManager.StaticData.HeroData.HeroTypeById.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel());
            return new StaticHeroTypeData() { HeroTypes = heroTypes };
        });

        EnsureTypesRead(ArtifactTypesKey, () =>
        {
            var artifactTypes = scope.StaticDataManager.StaticData.ArtifactData._setInfoByKind.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel());
            return new StaticArtifactData { ArtifactSetKinds = artifactTypes.ToModel() };
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

        EnsureTypesRead(SkillTypesKey, () =>
        {
            var skillTypes = scope.StaticDataManager.StaticData.SkillData._skillTypeById.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel());
            return new StaticSkillData() { SkillTypes = skillTypes };
        });

        HasWork = false;
        return Task.CompletedTask;
    }
}
