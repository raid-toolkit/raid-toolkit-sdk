using System;
using System.Threading.Tasks;

using Raid.Toolkit.Extensibility.Host;

using SuperSocket;
using SuperSocket.Channel;
using SuperSocket.WebSocket.Server;

namespace Raid.Toolkit.Application.Core.DependencyInjection
{
	public class SessionFactory : ISessionFactory
	{
		public SessionFactory()
		{
		}

		public Type SessionType => typeof(WebSocketSession);

		public IAppSession Create()
		{
			return new WebSocketSession();
		}
	}
}
