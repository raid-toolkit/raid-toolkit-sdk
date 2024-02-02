using Raid.Toolkit.Common.API;

using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace Raid.Toolkit.IPC;

public interface IPipeSession<T> : IDisposable
{
	bool IsConnected { get; }
	event EventHandler<T> MessageReceived;
	void StartListening(CancellationToken token = default);
	Task SendAsync(T message, CancellationToken cancellationToken = default);
}

public class IPCSession<T> : IPipeSession<T>, IApiSession<T>
{
	protected PipeStream Pipe;
	protected readonly IPCMessageSerializer<T> Serializer;
	protected CancellationToken ListenerCancellationToken;
	protected bool IsDisposed;

	public bool IsConnected => Pipe.IsConnected;
	public string Id { get; }
	public bool Connected => IsConnected;

	public event EventHandler<T>? MessageReceived;
	public event EventHandler? Closed;

	public IPCSession(PipeStream pipe, IPCMessageSerializer<T> serializer)
	{
		Id = Guid.NewGuid().ToString("n");
		Pipe = pipe;
		Serializer = serializer;
	}

	public virtual void StartListening(CancellationToken token = default)
	{
		ListenerCancellationToken = token;
		ThreadPool.QueueUserWorkItem(BeginReadMessage);
	}

	private async void BeginReadMessage(object? state)
	{
		T? message = await Serializer.ReadMessageAsync(Pipe, ListenerCancellationToken);
		if (message != null)
			MessageReceived?.Invoke(this, message);
		if (Pipe.IsConnected)
			ThreadPool.QueueUserWorkItem(BeginReadMessage);
	}

	public async Task SendAsync(T message, CancellationToken cancellationToken = default)
	{
		await Serializer.WriteMessageAsync(Pipe, message, cancellationToken);
		await Pipe.FlushAsync(cancellationToken);
		Pipe.WaitForPipeDrain();
	}

	public void Close()
	{
		Pipe.Close();
		Closed?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!IsDisposed)
		{
			if (disposing)
			{
				if (Pipe.IsConnected)
					Close();
				Pipe.Dispose();
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

}
