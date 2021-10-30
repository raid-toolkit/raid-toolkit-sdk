using Raid.DataModel;

namespace Raid.Service
{
    public interface IMessageScopeHandler
    {
        string Name { get; }
        void HandleMessage(SocketMessage message, ISocketSession session);
    }
}