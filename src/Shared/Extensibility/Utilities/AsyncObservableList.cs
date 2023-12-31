using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Raid.Toolkit.Extensibility.Utilities;

public class AsyncObservableList<T> : ObservableCollection<T>
{
    private readonly IAppDispatcher Dispatcher;
    public AsyncObservableList(IAppDispatcher dispatcher)
    {
        Dispatcher = dispatcher;
    }
    private readonly object _syncRoot = new();
    private ConcurrentQueue<NotifyCollectionChangedEventArgs>? eventQueue;
    private ConcurrentBag<string>? dirtyKeys;

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
    {
        lock (_syncRoot)
        {
            if (eventQueue == null)
            {
                ConcurrentQueue<NotifyCollectionChangedEventArgs> newQueue = new();
                newQueue.Enqueue(eventArgs);
                Dispatcher.Dispatch(() =>
                {
                    lock (_syncRoot)
                    {
                        eventQueue = null;
                    }
                    while (newQueue.TryDequeue(out NotifyCollectionChangedEventArgs? eventArgs))
                        base.OnCollectionChanged(eventArgs);
                });
                eventQueue = newQueue;
            }
            else
            {
                eventQueue.Enqueue(eventArgs);
            }
        }
    }

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
                        base.OnPropertyChanged(new(propertyName));
                });
                dirtyKeys = newBag;
            }
            else
            {
                dirtyKeys.Add(propertyName);
            }
        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs == null || string.IsNullOrEmpty(eventArgs.PropertyName))
            return;

        InvalidateProperty(eventArgs.PropertyName);
    }
}
