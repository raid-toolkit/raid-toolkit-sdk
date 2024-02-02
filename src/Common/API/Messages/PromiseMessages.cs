using System;
using System.ComponentModel;

using Newtonsoft.Json;

namespace Raid.Toolkit.Common.API.Messages;

public class PromiseMessage : AsyncMessage
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Exists for serialization only")]
	public PromiseMessage()
		: base()
	{
		Success = false;
	}

	public PromiseMessage(string promiseId, bool success)
		: base(promiseId)
	{
		Success = success;
	}

	[JsonProperty("success")]
	public bool Success;
}

public class PromiseSucceededMessage : PromiseMessage
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Exists for serialization only")]
	public PromiseSucceededMessage()
		: base()
	{
		Value = null;
	}

	public PromiseSucceededMessage(string promiseId)
		: base(promiseId, true)
	{
		Value = null;
	}

	public PromiseSucceededMessage(string promiseId, object? value)
		: base(promiseId, true)
	{
		Value = value;
	}

	[JsonProperty("value")]
	public object? Value;
}

public class PromiseFailedMessage : PromiseMessage
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Exists for serialization only")]
	public PromiseFailedMessage()
		: base(string.Empty, false)
	{
		ErrorInfo = new();
	}

	public PromiseFailedMessage(string promiseId, ErrorInfo errorInfo)
		: base(promiseId, false)
	{
		ErrorInfo = errorInfo;
	}

	[JsonProperty("error")]
	public ErrorInfo ErrorInfo;
}

public class ErrorInfo
{
	[JsonProperty("message")]
	public string Message;

	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Exists for serialization only")]
	public ErrorInfo()
	{
		Message = string.Empty;
	}
	public ErrorInfo(Exception ex)
	{
		Message = ex.Message;
	}
}
