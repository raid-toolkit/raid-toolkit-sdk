using System.Collections.Generic;
using Newtonsoft.Json;

namespace Raid.DataModel
{
    public class StatsSnapshot
    {
        [JsonIgnore]
        public Stats EffectiveStats = new();

        public float Health
        {
            get => EffectiveStats.Health;
            set => EffectiveStats.Health = value;
        }

        public float Attack
        {
            get => EffectiveStats.Attack;
            set => EffectiveStats.Attack = value;
        }

        public float Defense
        {
            get => EffectiveStats.Defense;
            set => EffectiveStats.Defense = value;
        }

        public float Speed
        {
            get => EffectiveStats.Speed;
            set => EffectiveStats.Speed = value;
        }

        public float Resistance
        {
            get => EffectiveStats.Resistance;
            set => EffectiveStats.Resistance = value;
        }

        public float Accuracy
        {
            get => EffectiveStats.Accuracy;
            set => EffectiveStats.Accuracy = value;
        }

        public float CriticalChance
        {
            get => EffectiveStats.CriticalChance;
            set => EffectiveStats.CriticalChance = value;
        }

        public float CriticalDamage
        {
            get => EffectiveStats.CriticalDamage;
            set => EffectiveStats.CriticalDamage = value;
        }

        public float CriticalHeal
        {
            get => EffectiveStats.CriticalHeal;
            set => EffectiveStats.CriticalHeal = value;
        }

        public Stats BaseStats;
        public IReadOnlyDictionary<StatSource, Stats> StatSources;
    }
}