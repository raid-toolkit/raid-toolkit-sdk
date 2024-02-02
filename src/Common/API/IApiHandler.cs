using Raid.Toolkit.Common.API.Messages;

namespace Raid.Toolkit.Common.API;

public interface IApiServer<T>
{
	bool SupportsScope(string scopeName);
	void HandleMessage(SocketMessage message, IApiSession<T> session);
}
