namespace Raid.Service.DataModel
{
    public class Stats
    {
        public float Health;
        public float Attack;
        public float Defense;
        public float Speed;
        public float Resistance;
        public float Accuracy;
        public float CriticalChance;
        public float CriticalDamage;
        public float CriticalHeal;
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