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
    }
}