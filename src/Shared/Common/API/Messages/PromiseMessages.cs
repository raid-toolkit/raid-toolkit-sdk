using System;
using Newtonsoft.Json;

namespace Raid.Toolkit.Common.API.Messages;

public class PromiseMessage : AsyncMessage
{
	[JsonProperty("success")]
	public bool Success;
}

public class PromiseSucceededMessage : PromiseMessage
{
	[JsonProperty("value")]
	public object Value = null;
}

public class PromiseFailedMessage : PromiseMessage
{
	[JsonProperty("error")]
	public ErrorInfo ErrorInfo;
}

public class ErrorInfo
{
	[JsonProperty("message")]
	public string Message = string.Empty;

	public ErrorInfo() { }
	public ErrorInfo(Exception ex)
	{
		Message = ex.Message;
	}
}
