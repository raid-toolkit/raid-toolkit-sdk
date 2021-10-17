using System.Collections.Generic;
using Newtonsoft.Json;
using SharedModel.Battle.Effects;

namespace Raid.Service.DataModel
{
    public class Stats : Dictionary<StatKindId, float>
    {
        public Stats()
        {
            foreach (StatKindId key in typeof(StatKindId).GetEnumValues())
            {
                this[key] = 0f;
            }
        }
        [JsonIgnore]
        public float Health
        {
            get => this[StatKindId.Health];
            set => this[StatKindId.Health] = value;
        }

        [JsonIgnore]
        public float Attack
        {
            get => this[StatKindId.Attack];
            set => this[StatKindId.Attack] = value;
        }

        [JsonIgnore]
        public float Defense
        {
            get => this[StatKindId.Defence];
            set => this[StatKindId.Defence] = value;
        }

        [JsonIgnore]
        public float Speed
        {
            get => this[StatKindId.Speed];
            set => this[StatKindId.Speed] = value;
        }

        [JsonIgnore]
        public float Resistance
        {
            get => this[StatKindId.Resistance];
            set => this[StatKindId.Resistance] = value;
        }

        [JsonIgnore]
        public float Accuracy
        {
            get => this[StatKindId.Accuracy];
            set => this[StatKindId.Accuracy] = value;
        }

        [JsonIgnore]
        public float CriticalChance
        {
            get => this[StatKindId.CriticalChance];
            set => this[StatKindId.CriticalChance] = value;
        }

        [JsonIgnore]
        public float CriticalDamage
        {
            get => this[StatKindId.CriticalDamage];
            set => this[StatKindId.CriticalDamage] = value;
        }

        [JsonIgnore]
        public float CriticalHeal
        {
            get => this[StatKindId.CriticalHeal];
            set => this[StatKindId.CriticalHeal] = value;
        }
    }

    public enum StatSource
    {
        Base = 0,
        Masteries = 1,
        Gear = 2,
        GearSets = 3,
        Arena = 4,
        GreatHall = 5,
        Clan = 6
    }

    public class StatsSnapshot
    {
        [JsonIgnore]
        public Stats EffectiveStats = new();

        public float Health
        {
            get => EffectiveStats[StatKindId.Health];
            set => EffectiveStats[StatKindId.Health] = value;
        }

        public float Attack
        {
            get => EffectiveStats[StatKindId.Attack];
            set => EffectiveStats[StatKindId.Attack] = value;
        }

        public float Defense
        {
            get => EffectiveStats[StatKindId.Defence];
            set => EffectiveStats[StatKindId.Defence] = value;
        }

        public float Speed
        {
            get => EffectiveStats[StatKindId.Speed];
            set => EffectiveStats[StatKindId.Speed] = value;
        }

        public float Resistance
        {
            get => EffectiveStats[StatKindId.Resistance];
            set => EffectiveStats[StatKindId.Resistance] = value;
        }

        public float Accuracy
        {
            get => EffectiveStats[StatKindId.Accuracy];
            set => EffectiveStats[StatKindId.Accuracy] = value;
        }

        public float CriticalChance
        {
            get => EffectiveStats[StatKindId.CriticalChance];
            set => EffectiveStats[StatKindId.CriticalChance] = value;
        }

        public float CriticalDamage
        {
            get => EffectiveStats[StatKindId.CriticalDamage];
            set => EffectiveStats[StatKindId.CriticalDamage] = value;
        }

        public float CriticalHeal
        {
            get => EffectiveStats[StatKindId.CriticalHeal];
            set => EffectiveStats[StatKindId.CriticalHeal] = value;
        }

        public Stats BaseStats;
        // NB: Intentionally not using NumericDictionary
        public IReadOnlyDictionary<StatSource, Stats> StatSources;
    }

    public static partial class ModelExtensions
    {
        public static Stats ToModel(this SharedModel.Meta.Heroes.BattleStats stats)
        {
            return new()
            {
                Health = stats.Health.AsFloat(),
                Attack = stats.Attack.AsFloat(),
                Defense = stats.Defence.AsFloat(),
                Accuracy = stats.Accuracy.AsFloat(),
                Resistance = stats.Resistance.AsFloat(),
                Speed = stats.Speed.AsFloat(),
                CriticalChance = stats.CriticalChance.AsFloat(),
                CriticalDamage = stats.CriticalDamage.AsFloat(),
                CriticalHeal = stats.CriticalHeal.AsFloat(),
            };
        }
    }
}