using System;
using System.Threading.Tasks;
using Raid.Toolkit.Extensibility.Host;
using SuperSocket;
using SuperSocket.Channel;
using SuperSocket.WebSocket.Server;

namespace Raid.Toolkit
{
    public class SessionFactory : ISessionFactory
    {
        private readonly ISessionManager SessionManager;
        public SessionFactory(ISessionManager sessionManager)
        {
            SessionManager = sessionManager;
        }

        public Type SessionType => typeof(WebSocketSession);

        public IAppSession Create()
        {
            WebSocketSession session = new();
            session.Connected += OnConnected;
            session.Closed += OnClosed;
            return session;
        }

        private ValueTask OnClosed(object sender, CloseEventArgs e)
        {
            SessionManager.OnClosed();
            return ValueTask.CompletedTask;
        }

        private ValueTask OnConnected(object sender, EventArgs e)
        {
            SessionManager.OnConnected();
            return ValueTask.CompletedTask;
        }
    }
}