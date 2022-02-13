using System;
using System.Collections.Generic;
using System.Linq;
using Raid.DataModel;
using Raid.DataServices;
using StatKindId = SharedModel.Battle.Effects.StatKindId;

namespace Raid.Service.DataServices
{
    [DataType("arena", Version = "1.1")]
    public class ArenaDataObject : ArenaData
    {
    }

    public class ArenaProvider : DataProviderBase<AccountDataContext, ArenaDataObject>
    {
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

        public ArenaProvider(IDataResolver<AccountDataContext, CachedDataStorage<PersistedDataStorage>, ArenaDataObject> storage)
            : base(storage)
        {
        }

        public override bool Update(ModelScope scope, AccountDataContext context)
        {
            var staticBonusData = StaticBonusData.GetValue(scope);

            var userWrapper = scope.AppModel._userWrapper;
            var capitalLevels = userWrapper.Village.VillageData.CapitolBonusLevelByStatByElement;

            List<GreatHallBonus> ghBonus = new();
            foreach ((var element, var bonus) in capitalLevels)
            {
                if (bonus == null)
                {
                    continue;
                }
                List<StatBonus> bonuses = new();
                foreach ((var statKindId, var level) in bonus)
                {
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
            if (leagueBorders.MaxSection.HasValue && leagueBorders.MinSection.HasValue)
            {
                placement = tagArenaPoints > leagueBorders.MaxSection.Value
                    ? TagArenaPlacement.Promotion
                    : tagArenaPoints < leagueBorders.MaxSection.Value ? TagArenaPlacement.Demotion : TagArenaPlacement.Retain;
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
                LeagueId = (DataModel.Enums.ArenaLeagueId)arenaData.ArenaDefenseTeamSetup?.ArenaLeagueId,
                WeeklyStats = new()
                {
                    Total = arenaData.BattlesStartedThisWeek,
                    Losses = arenaData.LossesThisWeek,
                    Wins = arenaData.VictoriesThisWeek
                },
                DefenseHeroIds = arenaData.ArenaDefenseTeamSetup.HeroSlotSetups
                    .Select(setup => setup.InventoryHeroId)
                    .ToArray(),
            };

            return PrimaryProvider.Write(context, new ArenaDataObject
            {
                GreatHallBonuses = ghBonus,
                TagArena = tagArena,
                ClassicArena = classicArena,
            });
        }
    }
}
