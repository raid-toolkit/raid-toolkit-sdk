using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Raid.Toolkit.IPC;

public interface IPCMessageSerializer<T>
{
    public Task<T?> ReadMessageAsync(Stream stream, CancellationToken cancellation = default);
    public Task<int> WriteMessageAsync(Stream stream, T message, CancellationToken cancellation = default);
}
