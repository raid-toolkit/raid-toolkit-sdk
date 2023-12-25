using Raid.Toolkit.Common.API;

namespace Raid.Toolkit.Extensibility.Interfaces;

public interface IWorkerApplication
{
	public IPCApiClient Client { get; }
}
