using System.Collections.Generic;
using Newtonsoft.Json;

namespace Raid.Toolkit.DataModel
{
    public class StatsSnapshot
    {
        [JsonIgnore]
        public Stats EffectiveStats = new();

        public double Health
        {
            get => EffectiveStats.Health;
            set => EffectiveStats.Health = value;
        }

        public double Attack
        {
            get => EffectiveStats.Attack;
            set => EffectiveStats.Attack = value;
        }

        public double Defense
        {
            get => EffectiveStats.Defense;
            set => EffectiveStats.Defense = value;
        }

        public double Speed
        {
            get => EffectiveStats.Speed;
            set => EffectiveStats.Speed = value;
        }

        public double Resistance
        {
            get => EffectiveStats.Resistance;
            set => EffectiveStats.Resistance = value;
        }

        public double Accuracy
        {
            get => EffectiveStats.Accuracy;
            set => EffectiveStats.Accuracy = value;
        }

        public double CriticalChance
        {
            get => EffectiveStats.CriticalChance;
            set => EffectiveStats.CriticalChance = value;
        }

        public double CriticalDamage
        {
            get => EffectiveStats.CriticalDamage;
            set => EffectiveStats.CriticalDamage = value;
        }

        public double CriticalHeal
        {
            get => EffectiveStats.CriticalHeal;
            set => EffectiveStats.CriticalHeal = value;
        }

        public Stats BaseStats;
        public IReadOnlyDictionary<StatSource, Stats> StatSources;
    }
}
