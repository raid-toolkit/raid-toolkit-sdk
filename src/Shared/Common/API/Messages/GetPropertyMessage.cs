using Newtonsoft.Json;

using System;
using System.ComponentModel;

namespace Raid.Toolkit.Common.API.Messages;

public class GetPropertyMessage : AsyncMessage
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Exists for serialization only")]
	public GetPropertyMessage()
	{
		PropertyName = string.Empty;
	}

	public GetPropertyMessage(string promiseId,  string propertyName)
		: base(promiseId)
	{
		PropertyName = propertyName;
	}

	[JsonProperty("propertyName")]
	public string PropertyName;
}
