using Raid.Toolkit.Common.API;
using Raid.Toolkit.Common.API.Messages;

namespace Raid.Toolkit.Extensibility.Host;

public interface IServerApplication
{
	void RegisterApiServer(IApiServer<SocketMessage> server);
}
