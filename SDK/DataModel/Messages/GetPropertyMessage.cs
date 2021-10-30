using Newtonsoft.Json;

namespace Raid.Service.Messages
{
    public class GetPropertyMessage : AsyncMessage
    {
        [JsonProperty("propertyName")]
        public string PropertyName;
    }
}