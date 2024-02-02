using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.ComponentModel;

namespace Raid.Toolkit.Common.API.Messages;

public class CallMethodMessage : AsyncMessage
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Exists for serialization only")]
	public CallMethodMessage()
	{
		MethodName = string.Empty;
		Parameters = new();
	}

	public CallMethodMessage(string promiseId, string methodName, JArray parameters)
		: base(promiseId)
	{
		MethodName = methodName;
		Parameters = parameters;
	}

	[JsonProperty("methodName")]
	public string MethodName;

	[JsonProperty("args")]
	public JArray Parameters;
}
