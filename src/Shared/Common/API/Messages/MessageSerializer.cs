using System;
using System.Diagnostics;
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
		using StreamReader sr = new(stream, StreamEncoding, false, 1024, true);
		return await sr.ReadLineAsync();
	}

	public async Task<int> WriteMessageAsync(Stream stream, string outString, CancellationToken cancellation = default)
	{
		using StreamWriter sw = new(stream, StreamEncoding, 1024, true);
		await sw.WriteLineAsync(outString);
		return outString.Length + 2;
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
