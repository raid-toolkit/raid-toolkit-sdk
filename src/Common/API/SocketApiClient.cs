using System;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Raid.Toolkit.Common.API.Messages;

namespace Raid.Toolkit.Common.API;

public class SocketApiClient : ApiClientBase
{
	private readonly Memory<byte> Buffer = new(new byte[1024 * 1024 * 16]);
	private static readonly ApiMessageSerializer Serializer = new();
	private readonly ClientWebSocket Socket = new();
	private readonly Uri EndpointUri;
	private readonly CancellationTokenSource CancellationTokenSource = new();
	public SocketApiClient(Uri? endpointUri = null) : base()
	{
		EndpointUri = endpointUri ?? new Uri("ws://localhost:9090");
	}

	public override async Task ConnectAsync(CancellationToken token = default)
	{
		if (Socket.State == WebSocketState.None)
		{
			await Socket.ConnectAsync(EndpointUri, token);
			ThreadPool.QueueUserWorkItem(BeginReadMessage);
		}
	}

	private async void BeginReadMessage(object? state)
	{
		var segment = new ArraySegment<byte>(Buffer.ToArray());
		var result = await Socket.ReceiveAsync(segment, CancellationTokenSource.Token);
		if (result.EndOfMessage)
		{
			using MemoryStream ms = new(segment.ToArray());
			ms.Seek(0, SeekOrigin.Begin);
			SocketMessage? message = await Serializer.ReadMessageAsync(ms, CancellationTokenSource.Token);
			if (message != null)
				HandleMessage(message);
		}

		if (Socket.State == WebSocketState.Open)
			ThreadPool.QueueUserWorkItem(BeginReadMessage);
	}

	public override void Disconnect()
	{
		CancellationTokenSource.Cancel();
		Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None).Wait();
	}

	protected override async Task SendAsync(SocketMessage message, CancellationToken cancellation = default)
	{
		using MemoryStream ms = new();
		await Serializer.WriteMessageAsync(ms, message, cancellation);
		ms.Seek(0, SeekOrigin.Begin);

		await Socket.SendAsync(new(ms.ToArray()), WebSocketMessageType.Text, true, cancellation);
	}
}
