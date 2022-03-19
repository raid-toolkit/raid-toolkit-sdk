using Newtonsoft.Json;

namespace Raid.Toolkit.DataModel
{
    public class GetPropertyMessage : AsyncMessage
    {
        [JsonProperty("propertyName")]
        public string PropertyName;
    }
}
