using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Services
{
    public interface IScopedServiceManager
    {
        public ValueTask ProcessMessage(ISocketSession session, string message);
        public void AddMessageScopeHandler(IMessageScopeHandler handler);
        public void RemoveMessageScopeHandler(IMessageScopeHandler handler);
    }
}
