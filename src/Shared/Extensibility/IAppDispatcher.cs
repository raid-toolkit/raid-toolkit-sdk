using System.Threading.Tasks;
using System.Threading;
using System;

namespace Raid.Toolkit.Extensibility;

public interface IAppDispatcher
{
	SynchronizationContext? SynchronizationContext { get; }
	void Dispatch(Action task);
	void Dispatch<TState>(Action<TState> action, TState state);
	Task Post(Action action);
	Task<T> Post<T>(Func<T> action);
	Task<T> Post<T>(Func<Task<T>> action);
	Task<T> Post<T, U>(Func<U, T> action, U state);
}
