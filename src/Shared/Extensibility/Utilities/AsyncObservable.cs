using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Raid.Toolkit.Extensibility.Utilities;

public class AsyncObservable : Observable
{
    private readonly IAppDispatcher Dispatcher;
    public AsyncObservable(IAppDispatcher dispatcher)
    {
        Dispatcher = dispatcher;
    }
    private readonly object _syncRoot = new();
    private ConcurrentBag<string>? dirtyKeys;

    private void InvalidateProperty(string propertyName)
    {
        lock (_syncRoot)
        {
            if (dirtyKeys == null)
            {
                ConcurrentBag<string> newBag = new() { propertyName };
                Dispatcher.Dispatch(() =>
                {
                    lock (_syncRoot)
                    {
                        dirtyKeys = null;
                    }
                    while (newBag.TryTake(out string? propertyName))
                        base.OnPropertyChanged(propertyName);
                });
                dirtyKeys = newBag;
            }
            else
            {
                dirtyKeys.Add(propertyName);
            }
        }
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        if (propertyName == null)
            return;

        InvalidateProperty(propertyName);
    }
}
