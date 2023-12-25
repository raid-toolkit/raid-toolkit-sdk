using Raid.Toolkit.Common.API.Messages;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Raid.Toolkit.Common.API;

public class ApiCallerBase<T>
{
	private readonly ApiClientBase Client;
	private readonly PublicApiInfo<T> Api = new();

	protected ApiCallerBase(ApiClientBase client)
	{
		Client = client;
		Client.EventReceived += Client_EventReceived;
	}

	private void Client_EventReceived(object sender, ApiClientEventArgs e)
	{
		HandleEvent(e.Message);
	}

	protected virtual void HandleEvent(SendEventMessage message)
	{
		var def = Api.GetMember<EventInfo>(message.EventName);
		if (def.MemberInfo is not EventInfo eventInfo)
			return;
		eventInfo.RaiseMethod.Invoke(this, new object[] { this, EventArgs.Empty });
	}

	protected Task<U> CallMethod<U>(MethodBase method, params object[] args)
	{
		var def = Api.GetMember<MethodInfo>(method.Name);
		return Client.Call<U>(def.Scope, def.Attribute.Name, args);
	}

	protected void Subscribe(EventInfo eventInfo)
	{
		var def = Api.GetMember<EventInfo>(eventInfo.Name);
		Client.Subscribe(def.Scope, def.Attribute.Name);
	}
}
