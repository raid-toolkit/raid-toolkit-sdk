using System;
using Newtonsoft.Json;
using System.Net.WebSockets;
using Raid.Extractor;
using System.Threading;
using System.Threading.Tasks;
using Raid.DataModel;
using System.Text;
using Raid.Service.Messages;
using Newtonsoft.Json.Linq;

namespace Raid.Client
{
    public class RaidToolkitClient
    {
        private readonly PromiseStore Promises = new();
        private readonly ClientWebSocket Socket = new();
        private readonly Uri EndpointUri;
        private CancellationTokenSource CancellationTokenSource = new();

        public RaidToolkitClient(Uri endpointUri = null) => EndpointUri = (endpointUri ?? new Uri("ws://localhost:9090"));

        public IAccountApi AccountApi => new AccountApi(this);
        public IStaticDataApi StaticDataApi => new StaticDataApi(this);

        public void Connect()
        {
            if (Socket.State == WebSocketState.None)
            {
                Socket.ConnectAsync(new Uri("ws://localhost:9090"), CancellationToken.None).Wait();
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
            Memory<byte> buffer = new Memory<byte>(new byte[1024 * 1024 * 3]);
            while (Socket.State == WebSocketState.Open)
            {
                var result = await Socket.ReceiveAsync(buffer, CancellationTokenSource.Token);
                if (!result.EndOfMessage)
                {
                    // TODO: throw away messages until next EndOfMessage is reached (inclusive)
                    continue;
                }
                var socketMessage = JsonConvert.DeserializeObject<SocketMessage>(Encoding.UTF8.GetString(buffer.Slice(0, result.Count).Span));
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
            }
        }

        private void Resolve(JToken message)
        {
            var promiseMsg = message.ToObject<PromiseMessage>();
            if (promiseMsg.Success)
            {
                Promises.Complete(promiseMsg.PromiseId, message.ToObject<PromiseSuccededMessage>().Value);
            }
            else
            {
                Promises.Fail(promiseMsg.PromiseId, message.ToObject<PromiseFailedMessage>().ErrorInfo);
            }
        }

        internal async Task<T> Call<T>(string apiName, string methodName, params object[] args)
        {
            string promiseId = Promises.Create();
            await Send(new SocketMessage()
            {
                Scope = apiName,
                Channel = "call",
                Message = JObject.FromObject(new CallMethodMessage()
                {
                    PromiseId = promiseId,
                    MethodName = methodName,
                    Parameters = JArray.FromObject(args)
                })
            });
            return await Promises.GetTask<T>(promiseId);
        }

        private async Task Send(SocketMessage message)
        {
            await Socket.SendAsync(
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)).AsMemory(),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}