using System.Threading;
using System.Threading.Tasks;

using Raid.Toolkit.Common.API;
using Raid.Toolkit.Common.API.Messages;

namespace Raid.Toolkit.Extensibility.Services
{
	public interface ISocketSession
	{
		string Id { get; }
		bool Connected { get; }
		Task Send(SocketMessage message, CancellationToken token = default);
	}
	public record SocketSessionAdapter(ISocketSession ApiSession) : IApiSession<SocketMessage>, ISocketSession
	{
		public string Id => ApiSession.Id;
		public bool Connected => ApiSession.Connected;
		public Task SendAsync(SocketMessage message, CancellationToken token = default) => ApiSession.Send(message, token);

		string ISocketSession.Id => ApiSession.Id;
		bool ISocketSession.Connected => ApiSession.Connected;
		Task ISocketSession.Send(SocketMessage message, CancellationToken token) => ApiSession.Send(message, token);
	}
}
