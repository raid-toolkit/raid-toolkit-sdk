using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility.Services;
using SuperSocket.WebSocket.Server;

namespace Raid.Toolkit
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
