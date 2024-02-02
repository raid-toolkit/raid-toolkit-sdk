using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.ComponentModel;
using System;

namespace Raid.Toolkit.Common.API.Messages;

public class SendEventMessage
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Exists for serialization only")]
	public SendEventMessage()
	{
		EventName = string.Empty;
		Payload = new();
	}

	public SendEventMessage(string eventName, JArray payload)
	{
		EventName = eventName;
		Payload = payload;
	}

	[JsonProperty("eventName")]
	public string EventName;

	[JsonProperty("payload")]
	public JArray Payload;
}
