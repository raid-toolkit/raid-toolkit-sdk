using Raid.Toolkit.DataModel;

namespace Raid.Toolkit.Extensibility.Services
{
    public interface IMessageScopeHandler
    {
        bool SupportsScope(string scopeName);
        void HandleMessage(SocketMessage message, ISocketSession session);
    }
}
