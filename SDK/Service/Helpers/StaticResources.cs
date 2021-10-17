using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SharedModel.Meta.Heroes;

namespace Raid.Service
{
    public static class StaticResources
    {
        public struct Multiplier
        {
            public HeroGrade stars;
            public int level;
            public float multiplier;
        }
        public static readonly Multiplier[] Multipliers;
        public static readonly Dictionary<HeroGrade, Dictionary<int, float>> MultiplierLookup;

        static StaticResources()
        {
            Multipliers = Deserialize<Multiplier[]>("Multipliers.json");
            MultiplierLookup = Multipliers
                .GroupBy(mult => mult.stars)
                .ToDictionary(
                    grp => grp.Key,
                    grp => grp.ToDictionary(lvl2Mult => lvl2Mult.level, lvl2Mult => lvl2Mult.multiplier)
                );
        }

        private static T Deserialize<T>(string resourceName)
        {
            var serializer = new JsonSerializer();
            var assembly = typeof(Program).Assembly;

            using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{resourceName}"))
            using (var sr = new StreamReader(stream))
            using (var textReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(textReader);
            }
        }

    }
}