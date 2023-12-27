using Newtonsoft.Json;

using Raid.Toolkit.Common.API;
using Raid.Toolkit.Common.API.Messages;
using SuperSocket.WebSocket.Server;

namespace Raid.Toolkit.Application.Core.DependencyInjection
{
    public class SuperSocketAdapter : IApiSession<SocketMessage>
    {
        private readonly WebSocketSession Session;
        public SuperSocketAdapter(WebSocketSession session) => Session = session;

        public string Id => Session.SessionID;
        public bool Connected => Session.State == SuperSocket.SessionState.Connected;

        public async Task SendAsync(SocketMessage message)
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
