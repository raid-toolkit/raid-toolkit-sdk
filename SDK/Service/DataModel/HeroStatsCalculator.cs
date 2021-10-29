using System.Linq;
using System.Collections.Generic;
using Raid.DataModel;
using SharedModel.Battle.Effects;
using System;

namespace Raid.Service
{
    public class HeroStatsCalculator
    {
        private HashSet<MasteryKindId> Masteries = new();
        private bool HasLoreOfSteel => Masteries.Contains(MasteryKindId.LoreofSteel);
        public readonly StatsSnapshot Snapshot = new()
        {
            BaseStats = new(),
            StatSources = new Dictionary<StatSource, Stats>()
            {
                {StatSource.Base, new()},
                {StatSource.Arena, new()},
                {StatSource.GreatHall, new()},
                {StatSource.Gear, new()},
                {StatSource.GearSets, new()},
                {StatSource.Masteries, new()},
                {StatSource.Clan, new()},
            }
        };

        public HeroStatsCalculator(Raid.DataModel.HeroType heroType, int rank, int level)
        {
            foreach (StatKindId statKind in typeof(StatKindId).GetEnumValues())
            {
                float baseStat = GetBaseStat(statKind, rank, level, heroType.UnscaledStats.GetStat(statKind));
                Snapshot.EffectiveStats.SetStat(statKind, baseStat);
                Snapshot.BaseStats.SetStat(statKind, baseStat);
                Snapshot.StatSources[StatSource.Base].SetStat(statKind, baseStat);
            }
        }

        public void AddStat(StatSource source, StatKindId statKind, float value)
        {
            Snapshot.EffectiveStats.AddStat(statKind, value);
            Snapshot.StatSources[source].AddStat(statKind, value);
        }

        public void ApplyMasteries(IEnumerable<MasteryKindId> masteries)
        {
            foreach (MasteryKindId mastery in masteries)
            {
                Masteries.Add(mastery);
                switch (mastery)
                {
                    case MasteryKindId.BladeDisciple:
                        AddStat(StatSource.Masteries, StatKindId.Attack, 75);
                        continue;
                    case MasteryKindId.DeadlyPrecision:
                        AddStat(StatSource.Masteries, StatKindId.CriticalChance, 5);
                        continue;
                    case MasteryKindId.KeenStrike:
                        AddStat(StatSource.Masteries, StatKindId.CriticalDamage, 10);
                        continue;
                    case MasteryKindId.FlawlessExecution:
                        AddStat(StatSource.Masteries, StatKindId.CriticalDamage, 20);
                        continue;
                    case MasteryKindId.ToughSkin:
                        AddStat(StatSource.Masteries, StatKindId.Defence, 75);
                        continue;
                    case MasteryKindId.IronSkin:
                        AddStat(StatSource.Masteries, StatKindId.Defence, 200);
                        continue;
                    case MasteryKindId.Defiant:
                        AddStat(StatSource.Masteries, StatKindId.Resistance, 10);
                        continue;
                    case MasteryKindId.Unshakeable:
                        AddStat(StatSource.Masteries, StatKindId.Resistance, 50);
                        continue;
                    case MasteryKindId.PinpointAccuracy:
                        AddStat(StatSource.Masteries, StatKindId.Accuracy, 10);
                        continue;
                    case MasteryKindId.EagleEye:
                        AddStat(StatSource.Masteries, StatKindId.Accuracy, 50);
                        continue;
                }
            }
        }

        public void ApplyBonuses(StatSource source, params StatBonus[] bonuses)
        {
            foreach (var bonus in bonuses)
            {
                var statKindId = Enum.Parse<StatKindId>(bonus.KindId);
                AddStat(
                    source,
                    statKindId,
                    bonus.Absolute
                    ? GetAbsoluteStatIncrease(statKindId, bonus.Value)
                    : GetFactorStatIncrease(statKindId, bonus.Value)
                );
            }
        }

        public void ApplyArtifactSetBonuses(float numberOfSets, params StatBonus[] bonuses)
        {
            foreach (var bonus in bonuses)
            {
                var statKindId = Enum.Parse<StatKindId>(bonus.KindId);
                AddStat(
                    StatSource.GearSets,
                    statKindId,
                    bonus.Absolute
                    ? GetAbsoluteStatIncrease(statKindId, numberOfSets * bonus.Value)
                    : GetFactorStatIncrease(statKindId, numberOfSets * bonus.Value * (HasLoreOfSteel ? 1.15f : 1))
                );
            }
        }

        public void ApplyArtifactBonuses(ArtifactStatBonus bonus)
        {
            var statKindId = Enum.Parse<StatKindId>(bonus.KindId);
            var value = bonus.Value + (float)bonus.GlyphPower;
            AddStat(
                StatSource.Gear,
                statKindId,
                bonus.Absolute
                ? GetAbsoluteStatIncrease(statKindId, value)
                : GetFactorStatIncrease(statKindId, value)
            );
        }

        public void ApplyArtifacts(IEnumerable<Artifact> artifacts)
        {
            foreach (var artifact in artifacts)
            {
                ApplyArtifactBonuses(artifact.PrimaryBonus);
                if (artifact.SecondaryBonuses != null)
                {
                    foreach (var bonus in artifact.SecondaryBonuses)
                        ApplyArtifactBonuses(bonus);
                }
            }
        }
        public void applyArenaStats(Stats battleStats)
        {
            foreach (StatKindId statKindId in Enum.GetValues<StatKindId>())
            {
                AddStat(StatSource.Arena, statKindId, GetArenaStatIncrease(statKindId, battleStats.GetStat(statKindId)));
            }
        }

        private static float GetBaseStat(StatKindId statKind, int rank, int level, float rawValue)
        {
            switch (statKind)
            {
                case StatKindId.Health:
                    return StaticResources.MultiplierLookup[rank][level] * rawValue * 15.003713524358135f;
                case StatKindId.Attack:
                case StatKindId.Defence:
                    return StaticResources.MultiplierLookup[rank][level] * rawValue;
                default:
                    return rawValue;
            }
        }

        private static float GetAbsoluteStatIncrease(StatKindId statKind, float rawValue)
        {
            switch (statKind)
            {
                case StatKindId.CriticalChance:
                case StatKindId.CriticalDamage:
                case StatKindId.CriticalHeal:
                    return rawValue * 100;
                default:
                    return rawValue;
            }
        }

        private float GetFactorStatIncrease(StatKindId statKind, float rawValue)
        {
            switch (statKind)
            {
                case StatKindId.CriticalChance:
                case StatKindId.CriticalDamage:
                case StatKindId.CriticalHeal:
                    return rawValue * 100;
                default:
                    return Snapshot.BaseStats.GetStat(statKind) * rawValue;
            }
        }

        private float GetArenaStatIncrease(StatKindId statKind, float rawValue)
        {
            switch (statKind)
            {
                case StatKindId.Health:
                case StatKindId.Attack:
                case StatKindId.Defence:
                    return GetFactorStatIncrease(statKind, rawValue);
                default:
                    return rawValue;
            }
        }
    }
}