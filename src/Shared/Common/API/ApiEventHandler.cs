using System;

namespace Raid.Toolkit.Common.API;

public class ApiEventHandler<T> where T : EventArgs
{
	private event EventHandler<T>? Sink;
	public static ApiEventHandler<T> operator +(ApiEventHandler<T> left, EventHandler<T> right)
	{
		left.Sink += right;
		return left;
	}
	public static ApiEventHandler<T> operator -(ApiEventHandler<T> left, EventHandler<T> right)
	{
		left.Sink -= right;
		return left;
	}

	public void Invoke(object? sender, T args)
	{
		Sink?.Invoke(sender, args);
	}
}

public class ApiEventHandler2<TEventArgs>
	where TEventArgs : EventArgs
{
	private readonly ApiClientBase Client;
	private readonly string Scope;
	private readonly string EventName;
	public ApiEventHandler2(ApiClientBase client, ApiMemberDefinition eventDefinition)
	{
		Client = client;
		Scope = eventDefinition.Scope;
		EventName = eventDefinition.Name;
	}

	public event EventHandler<TEventArgs>? Event;

	public void Invoke(object? sender, EventArgs e)
	{
		Event?.Invoke(sender, (TEventArgs)e);
	}

	public void Subscribe(EventHandler<TEventArgs> callback)
	{
		Client.Subscribe(Scope, EventName);
		Event += callback;
	}
	public void Unsubscribe(EventHandler<TEventArgs> callback)
	{
		Event -= callback;
		if ((Event?.GetInvocationList().Length ?? 0) == 0)
			Client.Unsubscribe(Scope, EventName);
	}
}
