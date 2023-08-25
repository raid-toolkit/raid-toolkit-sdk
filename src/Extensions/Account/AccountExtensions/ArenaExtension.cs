using Client.Model.Gameplay.Artifacts;
using StatKindId = SharedModel.Battle.Effects.StatKindId;

using Il2CppToolkit.Runtime;

using Microsoft.Extensions.Logging;
using Raid.Toolkit.Common;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extension.Account;

public class ArenaExtension :
    AccountDataExtensionBase,
    IAccountPublicApi<IGetAccountDataApi<ArenaData>>,
    IGetAccountDataApi<ArenaData>,
    IAccountExportable
{
    private const string Key = "arena.json";

    IGetAccountDataApi<ArenaData> IAccountPublicApi<IGetAccountDataApi<ArenaData>>.GetApi() => this;
    bool IGetAccountDataApi<ArenaData>.TryGetData(out ArenaData data) => Storage.TryRead(Key, out data);

    public ArenaExtension(IAccount account, IExtensionStorage storage, ILogger<ArenaExtension> logger)
    : base(account, storage, logger)
    {
    }

    protected override Task Update(ModelScope scope)
    {
        var staticBonusData = StaticBonusData.GetValue(scope);

        var userWrapper = scope.AppModel._userWrapper;
        var capitalLevels = userWrapper.Village.VillageData.CapitolBonusLevelByStatByElement;

        List<GreatHallBonus> ghBonus = new();
        foreach (var kvp in capitalLevels)
        {
            var element = kvp.Key;
            var bonus = kvp.Value;
            if (bonus == null)
            {
                continue;
            }
            List<StatBonus> bonuses = new();
            foreach (var kvp2 in bonus)
            {
                var statKindId = kvp2.Key;
                var level = kvp2.Value;
                if (staticBonusData.TryGetValue(statKindId, out var bonusValues))
                {
                    bonuses.Add(bonusValues[level - 1]);
                }
            }
            ghBonus.Add(new()
            {
                Affinity = (DataModel.Enums.Element)element,
                Bonus = bonuses,
                Levels = bonus.UnderlyingDictionary.ToDictionary(kvp => (DataModel.Enums.StatKindId)kvp.Key, kvp => kvp.Value)
            });
        }

        var arenaData = userWrapper.Arena.ArenaData;
        var tagArenaData = userWrapper.Arena3x3.Data;

        long tagArenaPoints = tagArenaData.ArenaPoints;
        var leagueBorders = userWrapper.Arena3x3.League._leagueBorders;
        TagArenaPlacement placement = TagArenaPlacement.Unknown;
        if (leagueBorders != null)
        {
            if (leagueBorders.MaxSection.HasValue && leagueBorders.MinSection.HasValue)
            {
                placement = tagArenaPoints > leagueBorders.MaxSection.Value
                    ? TagArenaPlacement.Promotion
                    : tagArenaPoints < leagueBorders.MaxSection.Value ? TagArenaPlacement.Demotion : TagArenaPlacement.Retain;
            }
        }

        TagArenaData tagArena = new()
        {
            ArenaPoints = tagArenaPoints,
            LeagueId = (DataModel.Enums.ArenaLeagueId)tagArenaData.LeagueId,
            WeeklyStats = new()
            {
                Total = tagArenaData.BattlesStartedThisWeek,
                Losses = tagArenaData.LossesThisWeek,
                Wins = tagArenaData.VictoriesThisWeek
            },
            DefenseHeroIds = tagArenaData.Arena3X3DefenseTeamSetup
                .Select(team => team.HeroSlotSetups.Select(setup => setup.InventoryHeroId).ToArray())
                .ToArray(),
            Placement = placement
        };
        ClassicArenaData classicArena = new()
        {
            ArenaPoints = arenaData.ArenaPoints,
            LeagueId = (DataModel.Enums.ArenaLeagueId?)arenaData.ArenaDefenseTeamSetup?.ArenaLeagueId,
            WeeklyStats = new()
            {
                Total = arenaData.BattlesStartedThisWeek,
                Losses = arenaData.LossesThisWeek,
                Wins = arenaData.VictoriesThisWeek
            },
            DefenseHeroIds = arenaData.ArenaDefenseTeamSetup?.HeroSlotSetups
                .Select(setup => setup.InventoryHeroId)
                .ToArray() ?? Array.Empty<int>(),
        };

        Storage.Write(Key, new ArenaData
        {
            GreatHallBonuses = ghBonus,
            TagArena = tagArena,
            ClassicArena = classicArena,
        });
        return Task.CompletedTask;
    }

    public void Export(IAccountReaderWriter account)
    {
        if (Storage.TryRead(Key, out ArenaData data))
            account.Write(Key, data);
    }

    public void Import(IAccountReaderWriter account)
    {
        if (account.TryRead(Key, out ArenaData? data))
            Storage.Write(Key, data);
    }

    private static readonly LazyInitializer<Dictionary<StatKindId, List<StatBonus>>, ModelScope> StaticBonusData = new(scope =>
    {
        Dictionary<StatKindId, List<StatBonus>> staticBonusData = new();
        var runtimeData = scope.StaticDataManager.StaticData.VillageData.CapitolBonusByStatKind;
        foreach (StatKindId statKindId in Enum.GetValues(typeof(StatKindId)))
        {
            if (!runtimeData.TryGetValue(statKindId, out var bonusValues))
                continue;
            staticBonusData.Add(statKindId, bonusValues.Select(bonusValue => bonusValue.ToModel(statKindId)).ToList());
        }
        return staticBonusData;
    });

}
