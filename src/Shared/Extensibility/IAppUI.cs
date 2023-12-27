using System;
using System.Threading;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility;

public interface IAppUI : IDisposable
{
    SynchronizationContext? SynchronizationContext { get; }
    void Run();
    void Dispatch(Action task);
    void Dispatch<TState>(Action<TState> action, TState state);
    Task Post(Action action);
    Task<T> Post<T>(Func<T> action);
    Task<T> Post<T>(Func<Task<T>> action);
    Task<T> Post<T, U>(Func<U, T> action, U state);

    Task<bool> ShowExtensionInstaller(ExtensionBundle bundleToInstall);
    void ShowMain();
    void ShowSettings();
    void ShowExtensionManager();
}
