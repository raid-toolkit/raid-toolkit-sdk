using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Raid.Toolkit.Common;
using Raid.Toolkit.Common.API.Messages;

namespace Raid.Toolkit.DataModel
{
	public class RaidToolkitClientBase
	{
		private readonly PromiseStore Promises = new();
		private readonly ClientWebSocket Socket = new();
		private readonly Uri EndpointUri;
		private readonly CancellationTokenSource CancellationTokenSource = new();

		public RaidToolkitClientBase(Uri? endpointUri = null)
		{
			EndpointUri = endpointUri ?? new Uri("ws://localhost:9090");
		}

		public T MakeApi<T>()
		{
			return (T?)Activator.CreateInstance(typeof(T), this) ?? throw new InvalidOperationException($"Could not construct object of type {typeof(T).FullName}");
		}

		private readonly Memory<byte> Buffer = new(new byte[1024 * 1024 * 16]);

		public void Connect()
		{
			if (!RegistrySettings.IsInstalled)
			{
				throw new NotSupportedException("Raid Toolkit must be installed");
			}
			if (Socket.State == WebSocketState.None)
			{
				Socket.ConnectAsync(EndpointUri, CancellationToken.None).Wait();
				Listen();
			}
		}

		public void Disconnect()
		{
			CancellationTokenSource.Cancel();
			Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None).Wait();
		}

		private async void Listen()
		{
			while (Socket.State == WebSocketState.Open)
			{
				var segment = new ArraySegment<byte>(Buffer.ToArray());
				var result = await Socket.ReceiveAsync(segment, CancellationTokenSource.Token);
				if (!result.EndOfMessage)
				{
					// TODO: throw away messages until next EndOfMessage is reached (inclusive)
					continue;
				}
				var socketMessage = JsonConvert.DeserializeObject<SocketMessage>(Encoding.UTF8.GetString(((Memory<byte>)segment).Slice(0, result.Count).Span.ToArray()));
				if (socketMessage == null)
					continue; // TODO: Add logging for malformed message?

				HandleMessage(socketMessage);
			}
		}

		private void HandleMessage(SocketMessage socketMessage)
		{
			switch (socketMessage.Channel)
			{
				case "set-promise":
					{
						Resolve(socketMessage.Message);
						return;
					}
				default:
					break;
			}
		}

		private void Resolve(JToken message)
		{
			var promiseMsg = message.ToObjectOrThrow<PromiseMessage>();
			if (promiseMsg.Success)
			{
				Promises.Complete(promiseMsg.PromiseId, message.ToObjectOrThrow<PromiseSucceededMessage>().Value);
			}
			else
			{
				Promises.Fail(promiseMsg.PromiseId, message.ToObject<PromiseFailedMessage>()!.ErrorInfo.Message);
			}
		}

		internal async void Subscribe(string apiName, string eventName)
		{
			await Send(new SocketMessage(apiName, "sub", JObject.FromObject(new SubscriptionMessage(eventName))));
		}

		internal async void Unsubscribe(string apiName, string eventName)
		{
			await Send(new SocketMessage(apiName, "unsub", JObject.FromObject(new SubscriptionMessage(eventName))));
		}

		internal async Task<T> Call<T>(string apiName, string methodName, params object[] args)
		{
			string promiseId = Promises.Create();
			await Send(new SocketMessage(apiName, "call", JObject.FromObject(new CallMethodMessage(promiseId, methodName, JArray.FromObject(args)))));
			return await Promises.GetTask<T>(promiseId);
		}

		private async Task Send(SocketMessage message)
		{
			await Socket.SendAsync(
				new(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)).AsMemory().ToArray()),
				WebSocketMessageType.Text,
				true,
				CancellationToken.None);
		}
	}
}
