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
            var localizedStrings = StaticDataFacet.ReadValue(StaticDataCache.Instance).LocalizedStrings;
            if (localizedStrings.TryGetValue(key.Key, out string value))
                return value;
            return key.EnValue ?? key.DefaultValue;
        }
    }
}