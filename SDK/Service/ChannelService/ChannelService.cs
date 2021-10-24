using System;
using System.Collections.Generic;
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
using Raid.Service.DataModel;
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
            ConnectTask = Socket.ConnectAsync(HostUri, stoppingToken);
            await ConnectTask;
            await Run(stoppingToken);
            await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "SHUTDOWN", CancellationToken.None);
        }

        private async Task Run(CancellationToken token)
        {
            Memory<byte> buffer = new Memory<byte>(new byte[1024 * 1024 * 3]);
            while (!token.IsCancellationRequested)
            {
                var result = await Socket.ReceiveAsync(buffer, token);
                if (!result.EndOfMessage)
                {
                    // TODO: throw away messages until next EndOfMessage is reached (inclusive)
                    continue;
                }
                var sendMessage = JsonConvert.DeserializeObject<SendMessage>(Encoding.UTF8.GetString(buffer.Slice(0, result.Count).Span));
                HandleMessage(sendMessage);
            }
        }

        private async void HandleMessage(SendMessage message)
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
                if (message.Message.ToObject<string>() == "dump")
                {
                    Logger.LogInformation("Sending account dump via websocket");
                    var account = UserData.UserAccounts.FirstOrDefault();
                    var dump = Extractor.DumpAccount(account);
                    var response = new SendMessage()
                    {
                        ChannelId = message.ChannelId,
                        Message = JObject.FromObject(dump),
                    };
                    await SendAsync(response);
                }
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
            await ConnectTask;
            UnauthorizedMessage message = new()
            {
                ChannelId = channelId,
                Reason = "User rejected the request"
            };
            await SendAsync(message);
        }

        public async void Accept(string channelId, string origin)
        {
            await ConnectTask;
            AuthorizeMessage message = new()
            {
                ChannelId = channelId,
                Origin = origin
            };
            await SendAsync(message);
        }
    }
}