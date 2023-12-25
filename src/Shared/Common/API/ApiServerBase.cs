using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

using Raid.Toolkit.Common.API.Messages;

namespace Raid.Toolkit.Common.API;

public abstract class ApiServer<T> : IApiServer<SocketMessage>
{
	private readonly Dictionary<string, EventHandler<SerializableEventArgs>> EventHandlerDelegates = new();
	protected ILogger<ApiServer<T>> Logger;
	private readonly PublicApiInfo<T> Api = new();

	private readonly string[] SupportedScopes;

	public ApiServer(ILogger<ApiServer<T>> logger)
	{
		Logger = logger;

		List<Type> types = new() { GetType() };
		types.AddRange(GetType().GetInterfaces());


		List<string> supportedScopes = new();
		foreach (Type type in types)
		{
			PublicApiAttribute attr = type.GetCustomAttribute<PublicApiAttribute>(true);
			if (attr == null)
				continue;
			supportedScopes.Add(attr.Name);
		}
		SupportedScopes = supportedScopes.ToArray();
	}

	public bool SupportsScope(string scopeName)
	{
		return SupportedScopes.Contains(scopeName);
	}

	public void HandleMessage(SocketMessage message, IApiSession<SocketMessage> session)
	{
		switch (message.Channel)
		{
			case "call":
				CallMethod(message.Message.ToObjectOrThrow<CallMethodMessage>(), session);
				break;
			case "get":
				GetProperty(message.Message.ToObjectOrThrow<GetPropertyMessage>(), session);
				break;
			case "sub":
				Subscribe(message.Message.ToObjectOrThrow<SubscriptionMessage>(), session);
				break;
			case "unsub":
				Unsubscribe(message.Message.ToObjectOrThrow<SubscriptionMessage>(), session);
				break;
			default:
				break;
		}
	}

	private void Subscribe(SubscriptionMessage subscriptionMessage, IApiSession<SocketMessage> session)
	{
		try
		{
			EventInfo eventInfo = Api.GetPublicApi<EventInfo>(subscriptionMessage.EventName, out string scope);
			if (!EventHandlerDelegates.TryGetValue($"{session.Id}:{scope}:{subscriptionMessage.EventName}", out var handler))
			{
				handler = async (object sender, SerializableEventArgs args) => await SendEvent(eventInfo, session, args, scope);
				EventHandlerDelegates.Add($"{session.Id}:{scope}:{subscriptionMessage.EventName}", handler);
			}
			eventInfo.AddEventHandler(this, handler);
		}
		catch (Exception ex)
		{
			Logger.LogError(ApiError.ApiProxyException.EventId(), ex, "Failed to subscribe");
		}
	}

	private void Unsubscribe(SubscriptionMessage subscriptionMessage, IApiSession<SocketMessage> session)
	{
		try
		{
			EventInfo eventInfo = Api.GetPublicApi<EventInfo>(subscriptionMessage.EventName, out string scope);
			if (!EventHandlerDelegates.TryGetValue($"{session.Id}:{scope}:{subscriptionMessage.EventName}", out EventHandler<SerializableEventArgs> handler))
				return;

			eventInfo.RemoveEventHandler(this, handler);
		}
		catch (Exception ex)
		{
			Logger.LogError(ApiError.ApiProxyException.EventId(), ex, "Failed to unsubscribe");
		}
	}

	private async Task SendEvent(EventInfo eventInfo, IApiSession<SocketMessage> session, SerializableEventArgs args, string scope)
	{
		try
		{
			if (!session.Connected)
			{
				if (EventHandlerDelegates.Remove($"{session.Id}:{scope}:{args.EventName}", out EventHandler<SerializableEventArgs>? handler))
				{
					eventInfo.RemoveEventHandler(this, handler);
				}
				return;
			}

			SendEventMessage eventMsg = new(args.EventName, JArray.FromObject(args.EventArguments));
			SocketMessage message = new(scope, "send-event", JToken.FromObject(eventMsg));
			await session.SendAsync(message);
		}
		catch (Exception ex)
		{
			Logger.LogError(ApiError.ApiProxyException.EventId(), ex, "Failed to send event");
		}
	}

	private async void CallMethod(CallMethodMessage message, IApiSession<SocketMessage> session)
	{
		string scope = string.Empty;
		try
		{
			MethodInfo methodInfo = Api.GetPublicApi<MethodInfo>(message.MethodName, out scope);

			var methodParameters = methodInfo.GetParameters();
			if (methodParameters.Length < message.Parameters.Count)
				throw new TargetParameterCountException();

			object?[] args = new object[methodParameters.Length];
			for (int p = 0; p < methodParameters.Length; ++p)
			{
				if (p >= message.Parameters.Count)
				{
					if (!methodParameters[p].IsOptional)
						throw new TargetParameterCountException();
					args[p] = Type.Missing;
					continue;
				}

				args[p] = message.Parameters[p]?.ToObject(methodParameters[p].ParameterType);
			}

			object result = methodInfo.Invoke(this, args);
			var returnValue = await message.Resolve(result);
			var response = new SocketMessage(scope, "set-promise", returnValue);
			await session.SendAsync(response);
		}
		catch (Exception ex)
		{
			Logger.LogError(ApiError.ApiProxyException.EventId(), ex, "Api call failed");
			var response = new SocketMessage(scope, "set-promise", message.Reject(ex));
			await session.SendAsync(response);
		}
	}

	private async void GetProperty(GetPropertyMessage message, IApiSession<SocketMessage> session)
	{
		string scope = string.Empty;
		try
		{
			PropertyInfo propertyInfo = Api.GetPublicApi<PropertyInfo>(message.PropertyName, out scope);
			object result = propertyInfo.GetValue(this);
			var returnValue = await message.Resolve(result);
			var response = new SocketMessage(scope, "set-promise", returnValue);
			await session.SendAsync(response);
		}
		catch (Exception ex)
		{
			Logger.LogError(ApiError.ApiProxyException.EventId(), ex, "Api property access failed");
			var response = new SocketMessage(scope, "set-promise", message.Reject(ex));
			await session.SendAsync(response);
		}
	}
}
