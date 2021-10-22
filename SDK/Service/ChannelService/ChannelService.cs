using System;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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
    public class SendMessage
    {
        [JsonProperty("type")]
        public string Type = "send";

        [JsonProperty("channelId")]
        public string ChannelId;

        [JsonProperty("message")]
        public JToken Message;
    }
    public class ChannelService
    {
        public static ChannelService Instance = new ChannelService();

        private CancellationTokenSource CancelToken;
        private ClientWebSocket Socket = new ClientWebSocket();
        private Uri HostUri;
        private Task ConnectTask;

        public ChannelService()
        {
            string publicServer = AppConfiguration.Configuration.GetValue<string>("publicServer");
            HostUri = new Uri(publicServer);
            Start();
        }

        public void Start()
        {
            CancelToken = new();
            ConnectTask = Socket.ConnectAsync(HostUri, CancelToken.Token);
            Run(CancelToken.Token);
        }

        private async void Run(CancellationToken token)
        {
            Memory<byte> buffer = new Memory<byte>(new byte[1024 * 1024 * 3]);
            await ConnectTask;
            while (!token.IsCancellationRequested)
            {
                var result = await Socket.ReceiveAsync(buffer, token);
                if (!result.EndOfMessage)
                {
                    // TODO: throw away messages until next EndOfMessage is reached (inclusive)
                    continue;
                }
                var sendMessage = JsonConvert.DeserializeObject<SendMessage>(Encoding.UTF8.GetString(buffer.Slice(0, result.Count).Span));
                if (sendMessage?.Type != "send")
                {
                    continue;
                }
                if (sendMessage.Message.ToObject<string>() == "dump")
                {
                    var account = UserData.Instance.UserAccounts.FirstOrDefault();
                    var dump = Extractor.DumpAccount(account);
                    var response = new SendMessage()
                    {
                        ChannelId = sendMessage.ChannelId,
                        Message = JObject.FromObject(dump),
                    };
                    await Socket.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response)).AsMemory(), WebSocketMessageType.Text, true, CancelToken?.Token ?? CancellationToken.None);
                }
            }
        }

        public void Stop()
        {
            ConnectTask = null;
            CancelToken?.Cancel();
            Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "SHUTDOWN", CancellationToken.None);
        }

        public async void Accept(string channelId, string origin)
        {
            if (ConnectTask == null)
            {
                return;
            }
            await ConnectTask;
            if (Socket.State == WebSocketState.Aborted)
            {
                Start();
            }
            await ConnectTask;
            AuthorizeMessage message = new()
            {
                ChannelId = channelId,
                Origin = origin
            };
            await Socket.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)).AsMemory(), WebSocketMessageType.Text, true, CancelToken?.Token ?? CancellationToken.None);
        }
    }
}