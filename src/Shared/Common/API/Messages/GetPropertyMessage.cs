using Newtonsoft.Json;

namespace Raid.Toolkit.Common.API.Messages;

public class GetPropertyMessage : AsyncMessage
{
	[JsonProperty("propertyName")]
	public string PropertyName;
}
