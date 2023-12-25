using Newtonsoft.Json;

namespace Raid.Toolkit.Common.API.Messages;

public class SubscriptionMessage
{
	[JsonProperty("eventName")]
	public string EventName;
}
