using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Raid.Toolkit.DataModel
{
    public static class StaticResources
    {
        public struct Multiplier
        {
            public int stars;
            public int level;
            public double multiplier;
        }
        public static readonly Multiplier[] Multipliers;
        public static readonly Dictionary<int, Dictionary<int, double>> MultiplierLookup;
        private static readonly Dictionary<string, string> LocalizedStrings = new();

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

        public static void AddStrings(IReadOnlyDictionary<string, string> strings)
        {
            foreach (var kvp in strings)
            {
                LocalizedStrings.Add(kvp.Key, kvp.Value);
            }
        }

        public static string Localize(this LocalizedText key)
        {
            if (LocalizedStrings.TryGetValue(key.Key, out string value))
                return value;
            return key.LocalizedValue ?? key.DefaultValue;
        }

        public static string LocalizeByKey(string key)
        {
            if (LocalizedStrings.TryGetValue(key, out string value))
                return value;
            return null;
        }

        private static T Deserialize<T>(string resourceName)
        {
            var serializer = new JsonSerializer();
            var assembly = typeof(StaticResources).Assembly;

            using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{resourceName}"))
            using (var sr = new StreamReader(stream))
            using (var textReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(textReader);
            }
        }

    }
}
