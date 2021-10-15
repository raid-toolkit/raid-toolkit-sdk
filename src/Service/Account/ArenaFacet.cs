using System;
using System.Collections.Generic;
using System.Linq;
using Raid.Service.DataModel;
using SharedModel.Battle.Effects;

namespace Raid.Service
{
    [Facet("arena")]
    public class ArenaFacet : UserAccountFacetBase<ArenaData, ArenaFacet>
    {
        private static LazyInitializer<Dictionary<StatKindId, List<StatBonus>>, ModelScope> StaticBonusData = new(scope =>
        {
            Dictionary<StatKindId, List<StatBonus>> staticBonusData = new();
            var runtimeData = scope.StaticDataManager.StaticData.VillageData.CapitolBonusByStatKind;
            foreach (StatKindId statKindId in Enum.GetValues(typeof(StatKindId)))
            {
                if (!runtimeData.TryGetValue(statKindId, out var bonusValues))
                    continue;
                staticBonusData.Add(statKindId, bonusValues.Select(bonusValue => new StatBonus()
                {
                    KindId = statKindId,
                    Absolute = bonusValue._isAbsolute,
                    Value = bonusValue._value.AsFloat()
                }).ToList());
            }
            return staticBonusData;
        });

        protected override ArenaData Merge(ModelScope scope, ArenaData previous = null)
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
                ghBonus.Add(new() { Affinity = element, Bonus = bonuses });
            }

            return new ArenaData
            {
                LeagueId = userWrapper.Arena.ArenaData.ArenaDefenseTeamSetup.ArenaLeagueId,
                GreatHallBonuses = ghBonus
            };
        }
    }
}