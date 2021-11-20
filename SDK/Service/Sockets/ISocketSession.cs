using System.Threading.Tasks;
using Raid.DataModel;

namespace Raid.Service
{
    public interface ISocketSession
    {
        string Id { get; }
        bool Connected { get; }
        Task Send(SocketMessage message);
    }
}
