using System.Threading.Tasks;
using Raid.Toolkit.DataModel;

namespace Raid.Toolkit.Extensibility.Services
{
    public interface ISocketSession
    {
        string Id { get; }
        bool Connected { get; }
        Task Send(SocketMessage message);
    }
}
