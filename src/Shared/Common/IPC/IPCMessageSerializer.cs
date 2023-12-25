using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Raid.Toolkit.IPC;

public interface IPCMessageSerializer<T>
{
	Task<T?> ReadMessageAsync(Stream stream, CancellationToken cancellation = default);
	Task<int> WriteMessageAsync(Stream stream, T message, CancellationToken cancellation = default);
}
