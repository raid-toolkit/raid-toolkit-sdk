using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raid.DataModel;
using RaidExtractor.Core;

namespace Raid.Service
{
    public class AuthorizeMessage
    {
        [JsonProperty("type")]
        public string Type = "authorize";

        [JsonProperty("channelId")]
        public string ChannelId;

        [JsonProperty("origin")]
        public string Origin;
    }

    public class UnauthorizedMessage
    {
        [JsonProperty("type")]
        public string Type = "unauthorized";

        [JsonProperty("channelId")]
        public string ChannelId;

        [JsonProperty("reason")]
        public string Reason;
    }

    public class SendMessage
    {
        [JsonProperty("type")]
        public string Type = "send";

        [JsonProperty("channelId")]
        public string ChannelId;

        [JsonProperty("message")]
        public JToken Message;
    }

    public class ChannelSocketAdapter : ISocketSession
    {
        ClientWebSocket Session;
        SendMessage ReceivedMessage;
        public ChannelSocketAdapter(ClientWebSocket session, SendMessage receivedMessage) => (Session, ReceivedMessage) = (session, receivedMessage);

        public string Id => "ClientSocket";
        public bool Connected => Session.State == WebSocketState.Open;

        public async Task Send(SocketMessage message)
        {
            SendMessage sendMessage = new()
            {
                ChannelId = ReceivedMessage.ChannelId,
                Message = JToken.FromObject(message)
            };
            await Session.SendAsync(
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(sendMessage)).AsMemory(),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
    public class ChannelService : BackgroundService
    {
        private readonly ClientWebSocket Socket = new ClientWebSocket();
        private readonly Uri HostUri;
        private Task ConnectTask;
        private readonly UserData UserData;
        private readonly Extractor Extractor;
        private readonly ILogger<ChannelService> Logger;

        public ChannelService(ILogger<ChannelService> logger, IOptions<AppSettings> settings, UserData userData, Extractor extractor)
        {
            Logger = logger;
            HostUri = new Uri(settings.Value.PublicServer);
            UserData = userData;
            Extractor = extractor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Delay(-1, stoppingToken);
            }
            catch { }
            if (Socket.State == WebSocketState.Open)
            {
                await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "SHUTDOWN", CancellationToken.None);
            }
        }

        private Task EnsureConnected()
        {
            if (ConnectTask == null)
            {
                ConnectTask = Socket.ConnectAsync(HostUri, CancellationToken.None);
                Task.Run(Run);
            }
            return ConnectTask;
        }

        private async Task Run()
        {
            await ConnectTask;
            Memory<byte> buffer = new Memory<byte>(new byte[1024 * 1024 * 3]);
            while (Socket.State == WebSocketState.Open)
            {
                var result = await Socket.ReceiveAsync(buffer, CancellationToken.None);
                if (!result.EndOfMessage)
                {
                    // TODO: throw away messages until next EndOfMessage is reached (inclusive)
                    continue;
                }
                var sendMessage = JsonConvert.DeserializeObject<SendMessage>(Encoding.UTF8.GetString(buffer.Slice(0, result.Count).Span));
                HandleMessage(sendMessage);
            }
        }

        private void HandleMessage(SendMessage message)
        {
            using var scope = Logger.BeginScope($"[ChannelId = {message.ChannelId}, Type = {message.Type}]");
            Logger.LogInformation($"Processing message");

            if (message?.Type != "send")
            {
                Logger.LogError($"Message type not supported");
                return;
            }
            try
            {
                SocketMessage socketMessage = message.Message.ToObject<SocketMessage>();
                ModelService.HandleMessage(new ChannelSocketAdapter(Socket, message), socketMessage);
            }
            catch (Exception ex)
            {
                Logger.LogError(ServiceError.MessageHandlerFailure.EventId(), ex, "Failed to process message");
            }
        }

        private async Task SendAsync<T>(T value)
        {
            await Socket.SendAsync(
                        Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)).AsMemory(),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                        );
        }

        public async void Reject(string channelId)
        {
            await EnsureConnected();
            UnauthorizedMessage message = new()
            {
                ChannelId = channelId,
                Reason = "User rejected the request"
            };
            await SendAsync(message);
        }

        public async void Accept(string channelId, string origin)
        {
            await EnsureConnected();
            AuthorizeMessage message = new()
            {
                ChannelId = channelId,
                Origin = origin
            };
            await SendAsync(message);
        }
    }
}