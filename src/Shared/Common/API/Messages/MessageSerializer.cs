using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Raid.Toolkit.IPC;

namespace Raid.Toolkit.Common.API.Messages;

public class ApiStringSerializer : IPCMessageSerializer<string>
{
	private readonly UnicodeEncoding StreamEncoding = new();

	public async Task<string?> ReadMessageAsync(Stream stream, CancellationToken cancellation = default)
	{
		byte[] byteSize = new byte[sizeof(int)];
		await stream.ReadAsync(byteSize, cancellation);
		int len = BitConverter.ToInt32(byteSize, 0);
		if (len <= 0)
			return null;
		var inBuffer = new byte[len];
		await stream.ReadAsync(inBuffer.AsMemory(0, len), cancellation);
		return StreamEncoding.GetString(inBuffer);
	}

	public async Task<int> WriteMessageAsync(Stream stream, string outString, CancellationToken cancellation = default)
	{
		byte[] outBuffer = StreamEncoding.GetBytes(outString);
		int len = outBuffer.Length;
		await stream.WriteAsync(BitConverter.GetBytes(len), cancellation);
		await stream.WriteAsync(outBuffer.AsMemory(0, len), cancellation);
		await stream.FlushAsync(cancellation);

		return outBuffer.Length + 2;
	}
}

public class ApiMessageSerializer : IPCMessageSerializer<SocketMessage>
{
	private static readonly ApiStringSerializer StringSerializer = new();
	public async Task<SocketMessage?> ReadMessageAsync(Stream stream, CancellationToken cancellation = default)
	{
		string? message = await StringSerializer.ReadMessageAsync(stream, cancellation);
		if (message == null)
			return null;
		return JsonConvert.DeserializeObject<SocketMessage>(message);
	}

	public Task<int> WriteMessageAsync(Stream stream, SocketMessage message, CancellationToken cancellation = default)
	{
		string serializedMessage = JsonConvert.SerializeObject(message);
		return StringSerializer.WriteMessageAsync(stream, serializedMessage, cancellation);
	}
}
