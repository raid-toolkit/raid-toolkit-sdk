using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Services
{
    public interface IScopedServiceManager
    {
        string[] SupportedApis { get; }
        ValueTask ProcessMessage(ISocketSession session, string message);
        void AddMessageScopeHandler(IMessageScopeHandler handler);
        void RemoveMessageScopeHandler(IMessageScopeHandler handler);
    }
}
