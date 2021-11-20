using Raid.DataModel;

namespace Raid.Service
{
    public interface IMessageScopeHandler
    {
        bool SupportsScope(string scopeName);
        void HandleMessage(SocketMessage message, ISocketSession session);
    }
}
