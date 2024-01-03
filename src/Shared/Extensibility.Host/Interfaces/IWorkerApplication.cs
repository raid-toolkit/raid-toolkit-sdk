using Raid.Toolkit.Common.API;

namespace Raid.Toolkit.Extensibility.Host;

public interface IWorkerApplication
{
    public IPCApiClient Client { get; }
}
