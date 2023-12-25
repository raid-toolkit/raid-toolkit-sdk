using System;
using System.ComponentModel;

namespace Raid.Toolkit.Common.API.Messages;

public abstract class SerializableEventArgs : EventArgs
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Exists for serialization only")]
	public SerializableEventArgs()
	{
		EventName = string.Empty;
		EventArguments = Array.Empty<string>();
	}
	public SerializableEventArgs(string eventName, params string[] arguments)
	{
		EventName = eventName;
		EventArguments = arguments;
	}
	public string EventName { get; set; }
	public object[] EventArguments { get; set; }
}

public class BasicSerializableEventArgs : SerializableEventArgs
{
	public BasicSerializableEventArgs(string eventName, params string[] arguments)
		: base(eventName, arguments)
	{ }
}

public class AccountUpdatedEventArgs : SerializableEventArgs
{
	public AccountUpdatedEventArgs(string accountId)
		: base("updated", new string[] { accountId })
	{ }
}

public class ViewUpdatedEventArgs : SerializableEventArgs
{
	public ViewUpdatedEventArgs(string accountId, string viewKey)
		: base("view-changed", new string[] { accountId, viewKey })
	{ }
}
