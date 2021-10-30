using Newtonsoft.Json;

namespace Raid.Service.Messages
{
    public class SubscriptionMessage
    {
        [JsonProperty("eventName")]
        public string EventName;
    }
}