using System.IO;
using Newtonsoft.Json;

namespace Raid.Extractor
{
    public static class StaticResources
    {
        public struct Multiplier
        {
            public SharedModel.Meta.Heroes.HeroGrade stars;
            public int level;
            public float multiplier;
        }
        public static readonly Multiplier[] Multipliers;

        static StaticResources()
        {
            Multipliers = Deserialize<Multiplier[]>("Multipliers.json");
        }

        private static T Deserialize<T>(string resourceName)
        {
            var serializer = new JsonSerializer();
            var assembly = typeof(Extractor).Assembly;

            using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{resourceName}"))
            using (var sr = new StreamReader(stream))
            using (var textReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(textReader);
            }
        }

    }
}