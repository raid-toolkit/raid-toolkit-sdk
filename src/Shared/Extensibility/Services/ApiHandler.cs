using System;

using Microsoft.Extensions.Logging;

using Raid.Toolkit.Common.API;
using Raid.Toolkit.Common.API.Messages;

namespace Raid.Toolkit.Extensibility.Services
{
	[Obsolete("Use ApiServerBase instead")]
	public abstract class ApiHandler<T> : ApiServer<T>, IMessageScopeHandler
	{
		protected ApiHandler(ILogger<ApiServer<T>> logger) : base(logger) { }
		void IMessageScopeHandler.HandleMessage(SocketMessage message, ISocketSession session) => HandleMessage(message, new SocketSessionAdapter(session));
		bool IMessageScopeHandler.SupportsScope(string scopeName) => SupportsScope(scopeName);
	}
}
