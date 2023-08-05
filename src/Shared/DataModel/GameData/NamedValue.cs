using Newtonsoft.Json;

using System;

namespace Raid.Toolkit.DataModel
{
    public class NamedValue
    {
        [JsonProperty("name")]
        public LocalizedText Name;
    }
}
