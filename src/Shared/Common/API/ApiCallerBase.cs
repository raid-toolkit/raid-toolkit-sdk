using Raid.Toolkit.Common.API.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Raid.Toolkit.Common.API;

public class ApiCallerBase<T>
{
	protected readonly ApiClientBase Client;
	protected readonly PublicApiInfo<T> Api = new();

	protected ApiCallerBase(ApiClientBase client)
	{
		Client = client;
		Client.EventReceived += Client_EventReceived;
	}

	private void Client_EventReceived(object? sender, ApiClientEventArgs e)
	{
		if (e.ContractName != Api.Name)
			return;
		HandleEvent(e.Message);
	}

	private readonly Dictionary<string, List<Delegate>> EventHandlers = new();

	protected void AddHandler(string memberName, Delegate handler)
	{
		var def = Api.GetMember<EventInfo>(memberName);
		EventInfo eventInfo = (EventInfo)def.MemberInfo;
		if (!EventHandlers.TryGetValue(memberName, out List<Delegate>? handlers))
		{
			EventHandlers.Add(memberName, new List<Delegate>() { handler });
			Subscribe(eventInfo);
			return;
		}
		handlers.Add(handler);
	}

	protected void RemoveHandler(string memberName, Delegate handler)
	{
		var def = Api.GetMember<EventInfo>(memberName);
		EventInfo eventInfo = (EventInfo)def.MemberInfo;
		if (!EventHandlers.TryGetValue(memberName, out List<Delegate>? handlers))
			return;
		handlers.Remove(handler);
		if (handlers.Count == 0)
		{
			EventHandlers.Remove(memberName);
			Unsubscribe(eventInfo);
		}
	}

	protected virtual void HandleEvent(SendEventMessage message)
	{
		var def = Api.GetMemberByApiName<EventInfo>(message.EventName);
		if (def.MemberInfo is not EventInfo eventInfo || !EventHandlers.TryGetValue(message.EventName, out List<Delegate>? handlers))
			return;
		Type eventArgsType = eventInfo!.EventHandlerType!.GetMethod("Invoke")!.GetParameters()[1].ParameterType;
		object[] eventArgsValues = eventArgsType.GetConstructors()
			.First(ctor => ctor.GetParameters().Length == message.Payload.Count)
			.GetParameters()
			.Select(param => param.ParameterType)
			.Zip(message.Payload, (paramType, arg) => arg.ToObject(paramType)!)
			.ToArray();
		object eventArgs = Activator.CreateInstance(eventArgsType, eventArgsValues)!;
		foreach (Delegate handler in handlers)
			handler.DynamicInvoke(this, eventArgs);
	}

	[Obsolete("Use CallMethod(string,...) instead")]
	protected Task<U> CallMethod<U>(MethodBase method, params object[] args)
	{
		var def = Api.GetMember<MethodInfo>(method.Name);
		return Client.Call<U>(def.Scope, def.Attribute.Name, args);
	}

	[Obsolete("Use CallMethod(string,...) instead")]
	protected Task CallMethod(MethodBase method, params object[] args)
	{
		var def = Api.GetMember<MethodInfo>(method.Name);
		return Client.Call(def.Scope, def.Attribute.Name, args);
	}

	protected Task<U> CallMethod<U>(string methodName, params object[] args)
	{
		var def = Api.GetMember<MethodInfo>(methodName);
		return Client.Call<U>(def.Scope, def.Attribute.Name, args);
	}

	protected Task CallMethod(string methodName, params object[] args)
	{
		var def = Api.GetMember<MethodInfo>(methodName);
		return Client.Call(def.Scope, def.Attribute.Name, args);
	}

	protected void Subscribe(EventInfo eventInfo)
	{
		var def = Api.GetMember<EventInfo>(eventInfo.Name);
		Client.Subscribe(def.Scope, def.Attribute.Name);
	}

	protected void Unsubscribe(EventInfo eventInfo)
	{
		var def = Api.GetMember<EventInfo>(eventInfo.Name);
		Client.Unsubscribe(def.Scope, def.Attribute.Name);
	}
}
