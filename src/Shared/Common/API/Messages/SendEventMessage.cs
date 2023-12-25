using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Raid.Toolkit.Common.API.Messages;

public class SendEventMessage
{
	[JsonProperty("eventName")]
	public string EventName;

	[JsonProperty("payload")]
	public JArray Payload;
}
