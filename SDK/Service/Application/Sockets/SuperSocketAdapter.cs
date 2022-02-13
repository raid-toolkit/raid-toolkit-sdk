using Newtonsoft.Json;
using System.Threading.Tasks;
using SuperSocket.WebSocket.Server;
using Raid.DataModel;
using System;

namespace Raid.Service
{
    public class SuperSocketAdapter : ISocketSession
    {
        WebSocketSession Session;
        public SuperSocketAdapter(WebSocketSession session) => Session = session;

        public string Id => Session.SessionID;
        public bool Connected => Session.State == SuperSocket.SessionState.Connected;

        public async Task Send(SocketMessage message)
        {
            try
            {
                await Session.SendAsync(JsonConvert.SerializeObject(message));
            }
            catch (Exception)
            { }
        }
    }
}
