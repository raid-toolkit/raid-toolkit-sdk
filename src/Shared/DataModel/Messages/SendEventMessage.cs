using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Raid.Toolkit.DataModel
{
    public class SendEventMessage
    {
        [JsonProperty("eventName")]
        public string EventName;

        [JsonProperty("payload")]
        public JArray Payload;
    }
}
