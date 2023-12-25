using Raid.Toolkit.Common.API;
using Raid.Toolkit.Common.API.Messages;

namespace Raid.Toolkit.Extensibility.Interfaces;

public interface IServerApplication
{
	public void RegisterApiServer(IApiServer<SocketMessage> server);
}
