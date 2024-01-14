using System;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Raid.Toolkit.Common.API.Messages;

namespace Raid.Toolkit.Common.API;

public class ApiClientEventArgs : EventArgs
{
	public string ContractName { get; }
	public SendEventMessage Message { get; }
	public ApiClientEventArgs(string contractName, SendEventMessage message)
	{
		Message = message;
		ContractName = contractName;
	}
}

public abstract class ApiClientBase
{
	private readonly PromiseStore Promises = new();

	public event EventHandler<ApiClientEventArgs>? EventReceived;

	public abstract Task ConnectAsync(CancellationToken token = default);

	public abstract void Disconnect();

	public T MakeApi<T>() => (T)Activator.CreateInstance(typeof(T), this)!;

	protected abstract Task SendAsync(SocketMessage message, CancellationToken cancellation = default);

	protected void HandleMessage(SocketMessage socketMessage)
	{
		switch (socketMessage.Channel)
		{
			case "set-promise":
				Resolve(socketMessage.Message);
				return;
			case "send-event":
				Emit(socketMessage.Scope, socketMessage.Message);
				return;
			default:
				break;
		}
	}

	private void Emit(string contractName, JToken message)
	{
		EventReceived?.Invoke(this, new ApiClientEventArgs(contractName, message.ToObject<SendEventMessage>()!));
	}

	protected void Resolve(JToken message)
	{
		var promiseMsg = message.ToObject<PromiseMessage>()!;
		if (promiseMsg.Success)
		{
			Promises.Complete(promiseMsg.PromiseId, message.ToObjectOrThrow<PromiseSucceededMessage>().Value);
		}
		else
		{
			Promises.Fail(promiseMsg.PromiseId, message.ToObject<PromiseFailedMessage>()!.ErrorInfo.Message);
		}
	}

	public async void Subscribe(string apiName, string eventName)
	{
		await SendAsync(new SocketMessage(apiName, "sub", JObject.FromObject(new SubscriptionMessage(eventName))));
	}

	public async void Unsubscribe(string apiName, string eventName)
	{
		await SendAsync(new SocketMessage(apiName, "unsub", JObject.FromObject(new SubscriptionMessage(eventName))));
	}

	public async Task<T> Call<T>(string apiName, string methodName, params object[] args)
	{
		string promiseId = Promises.Create();
		await SendAsync(new SocketMessage(apiName, "call", JObject.FromObject(new CallMethodMessage(promiseId, methodName, JArray.FromObject(args)))));
		return await Promises.GetTask<T>(promiseId);
	}
	public async Task Call(string apiName, string methodName, params object[] args)
	{
		string promiseId = Promises.Create();
		await SendAsync(new SocketMessage(apiName, "call", JObject.FromObject(new CallMethodMessage(promiseId, methodName, JArray.FromObject(args)))));
		await Promises.GetTask(promiseId);
	}
}
