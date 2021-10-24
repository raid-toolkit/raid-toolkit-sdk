using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SharedModel.Common.Localization;

namespace Raid.Service.DataModel
{
    public class LocalizedText
    {
        [JsonProperty("key")]
        public string Key;

        [JsonProperty("defaultValue")]
        public string DefaultValue;

        [JsonProperty("EnValue")]
        public string EnValue;
    }

    public static partial class ModelExtensions
    {
        static class LocalizationStrings
        {
            public static IReadOnlyDictionary<string, string> LocalizedStrings = StaticDataFacet.ReadValue(RaidHost.Services.GetService<StaticDataCache>()).LocalizedStrings;
        }
        public static LocalizedText ToModel(this SharedLTextKey key)
        {
            return new()
            {
                Key = key.Key,
                DefaultValue = key.DefaultValue,
            };
        }
        public static string Localize(this LocalizedText key)
        {
            if (LocalizationStrings.LocalizedStrings.TryGetValue(key.Key, out string value))
                return value;
            return key.EnValue ?? key.DefaultValue;
        }
    }
}