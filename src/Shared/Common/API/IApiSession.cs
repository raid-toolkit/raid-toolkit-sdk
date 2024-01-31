using System.Threading;
using System.Threading.Tasks;

namespace Raid.Toolkit.Common.API;

public interface IApiSession<T>
{
	string Id { get; }
	bool Connected { get; }
	Task SendAsync(T message, CancellationToken token = default);
}
