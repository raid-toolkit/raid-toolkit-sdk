using System.Threading.Tasks;

using Raid.Toolkit.Common.API;
using Raid.Toolkit.Common.API.Messages;

namespace Raid.Toolkit.Extensibility.Services
{
	public interface ISocketSession
	{
		string Id { get; }
		bool Connected { get; }
		Task Send(SocketMessage message);
	}
	public record SocketSessionAdapter(ISocketSession ApiSession) : IApiSession<SocketMessage>, ISocketSession
	{
		public string Id => ApiSession.Id;
		public bool Connected => ApiSession.Connected;
		public Task SendAsync(SocketMessage message) => ApiSession.Send(message);

		string ISocketSession.Id => ApiSession.Id;
		bool ISocketSession.Connected => ApiSession.Connected;
		Task ISocketSession.Send(SocketMessage message) => ApiSession.Send(message);
	}
}
