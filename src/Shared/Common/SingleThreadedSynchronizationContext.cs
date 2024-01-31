using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Raid.Toolkit.Common;

public sealed class SingleThreadedSynchronizationContext : SynchronizationContext
{
    private readonly BlockingCollection<(SendOrPostCallback d, object? state)> queue = new();

    public override void Post(SendOrPostCallback d, object? state)
    {
        queue.Add((d, state));
    }

    public static void Await(Func<Task> taskinvoker)
    {
        var originalContext = Current;
        try
        {
            var context = new SingleThreadedSynchronizationContext();
            SetSynchronizationContext(context);

            var task = taskinvoker.Invoke();
            task.ContinueWith(_ => context.queue.CompleteAdding());

            while (context.queue.TryTake(out var work, Timeout.Infinite))
                work.d.Invoke(work.state);

            task.GetAwaiter().GetResult();
        }
        finally
        {
            SetSynchronizationContext(originalContext);
        }
    }
}
