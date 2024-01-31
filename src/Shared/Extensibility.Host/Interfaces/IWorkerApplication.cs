using Raid.Toolkit.Common.API;

namespace Raid.Toolkit.Extensibility.Host;

public interface IWorkerApplication
{
	IPCApiClient Client { get; }
}
