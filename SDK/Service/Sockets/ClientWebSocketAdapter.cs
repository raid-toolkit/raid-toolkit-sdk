using Newtonsoft.Json;
using Raid.DataModel;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Raid.Service
{
    public class ClientWebSocketAdapter : ISocketSession
    {
        ClientWebSocket Session;
        public ClientWebSocketAdapter(ClientWebSocket session) => Session = session;

        public string Id => "ClientSocket";
        public bool Connected => Session.State == WebSocketState.Open;

        public async Task Send(SocketMessage message)
        {
            await Session.SendAsync(
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)).AsMemory(),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}
