using System;
using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace Raid.Toolkit.IPC;

public class IPCServer<T> : IDisposable
{
	private readonly CancellationTokenSource Cancellation;
	private readonly ConcurrentBag<IPipeSession<T>> Connections = new();
	private readonly string PipeName;
	private readonly IPCMessageSerializer<T> Serializer;
	private bool IsDisposed;

	public event EventHandler<T>? MessageReceived;

	public IPCServer(string pipeName, IPCMessageSerializer<T> serializer)
	{
		PipeName = pipeName;
		Serializer = serializer;
		Cancellation = new();
	}

	public void Start()
	{
		CreatePendingConnection();
	}

	public void Stop()
	{
		Dispose();
	}

	public void CreatePendingConnection(object? _state = null)
	{
		IPCServerConnection connection = new(PipeName, Serializer);
		connection.MessageReceived += Connection_MessageReceived;
		Connections.Add(connection);
		connection.WaitForConnection().ContinueWith(CreatePendingConnection, Cancellation.Token);
	}

	private void Connection_MessageReceived(object? sender, T e)
	{
		MessageReceived?.Invoke(sender, e);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!IsDisposed)
		{
			if (disposing)
			{
				Cancellation.Cancel();
				foreach (IPipeSession<T> connection in Connections)
				{
					connection.Dispose();
				}
			}

			IsDisposed = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	internal class IPCServerConnection : IPCSession<T>
	{
		public IPCServerConnection(string pipeName, IPCMessageSerializer<T> serializer)
			: base(new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous), serializer)
		{ }

		public async Task WaitForConnection()
		{
			if (Pipe is not NamedPipeServerStream server)
				throw new InvalidCastException(nameof(Pipe));

			await server.WaitForConnectionAsync();
			StartListening();
		}
	}
}
