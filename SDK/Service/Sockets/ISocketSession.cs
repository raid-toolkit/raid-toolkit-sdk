using System.Threading.Tasks;

namespace Raid.Service
{
    public interface ISocketSession
    {
        string Id { get; }
        bool Connected { get; }
        Task Send(SocketMessage message);
    }
}