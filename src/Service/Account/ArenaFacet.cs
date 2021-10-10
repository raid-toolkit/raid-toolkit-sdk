using System;
using System.Collections.Generic;
using System.Linq;
using Raid.Service.DataModel;
using SharedModel.Battle.Effects;
using SharedModel.Meta.Artifacts;

namespace Raid.Service
{
    public class ArenaFacet : Facet<ArenaData>
    {
        public override string Id => "arena";

        private Dictionary<StatKindId, List<StatBonus>> m_staticBonusData;
        private Dictionary<StatKindId, List<StatBonus>> EnsureStaticBonusData(ModelScope scope)
        {
            if (m_staticBonusData == null)
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
                m_staticBonusData = staticBonusData;
            }
            return m_staticBonusData;
        }

        protected override ArenaData Merge(ModelScope scope, ArenaData previous = null)
        {
            var userWrapper = scope.AppModel._userWrapper;
            var capitalLevels = userWrapper.Village.VillageData.CapitolBonusLevelByStatByElement;
            var staticBonusData = EnsureStaticBonusData(scope);
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