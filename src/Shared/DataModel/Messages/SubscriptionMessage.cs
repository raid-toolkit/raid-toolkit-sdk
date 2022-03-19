using Newtonsoft.Json;

namespace Raid.Toolkit.DataModel
{
    public class SubscriptionMessage
    {
        [JsonProperty("eventName")]
        public string EventName;
    }
}
