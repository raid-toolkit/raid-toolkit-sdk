using Raid.Service.Messages;
using SuperSocket.WebSocket.Server;

namespace Raid.Service
{
    public interface IMessageScopeHandler
    {
        string Name { get; }
        void HandleMessage(SocketMessage message, WebSocketSession session);
    }
}