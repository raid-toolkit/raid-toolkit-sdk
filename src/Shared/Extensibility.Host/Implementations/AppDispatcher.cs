using System;
using System.Threading;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host;

public class AppDispatcher : IAppDispatcher, IDisposable
{
	private bool IsDisposed;
	public SynchronizationContext? SynchronizationContext { get; }

	public AppDispatcher()
	{
		SynchronizationContext = SynchronizationContext.Current;
	}

	#region Dispatch
	public void Dispatch(Action action)
	{
		if (SynchronizationContext.Current == SynchronizationContext)
			action();
		else
			SynchronizationContext?.Post(_ => action(), null);
	}

	public void Dispatch<TState>(Action<TState> action, TState state)
	{
		if (SynchronizationContext.Current == SynchronizationContext)
			action(state);
		else
			SynchronizationContext?.Post(state => action((TState)state!), state);
	}

	public Task Post(Action action)
	{
		TaskCompletionSource signal = new();
		Dispatch(() =>
		{
			try
			{
				action();
				signal.SetResult();
			}
			catch (Exception ex)
			{
				signal.SetException(ex);
			}
		});
		return signal.Task;
	}

	public Task<T> Post<T>(Func<T> action)
	{
		TaskCompletionSource<T> signal = new();
		Dispatch(() =>
		{
			try
			{
				T result = action();
				signal.SetResult(result);
			}
			catch (Exception ex)
			{
				signal.SetException(ex);
			}
		});
		return signal.Task;
	}

	public Task<T> Post<T>(Func<Task<T>> action)
	{
		TaskCompletionSource<T> signal = new();
		Dispatch(async () =>
		{
			try
			{
				T result = await action();
				signal.SetResult(result);
			}
			catch (Exception ex)
			{
				signal.SetException(ex);
			}
		});
		return signal.Task;
	}

	public Task<T> Post<T, U>(Func<U, T> action, U state)
	{
		TaskCompletionSource<T> signal = new();
		Dispatch(() =>
		{
			try
			{
				T result = action(state);
				signal.SetResult(result);
			}
			catch (Exception ex)
			{
				signal.SetException(ex);
			}
		});
		return signal.Task;
	}
	#endregion Dispatch


	protected virtual void Dispose(bool disposing)
	{
		if (!IsDisposed)
		{
			if (disposing)
			{
				// TODO: dispose managed state (managed objects)
			}

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null
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
