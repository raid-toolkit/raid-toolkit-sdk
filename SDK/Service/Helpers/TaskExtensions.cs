using System;
using System.Threading;
using System.Threading.Tasks;

namespace Raid.Service
{
    public static class TaskExtensions
    {
        private static bool shuttingDown = false;
        private static CancellationTokenSource cancellationTokenSource = new();
        public static void Shutdown()
        {
            shuttingDown = true;
            cancellationTokenSource.Cancel();
        }

        public static Task RunAfter(int millisecondsDelay, Action callback)
        {
            if (shuttingDown)
            {
                return null;
            }
            return Task.Delay(millisecondsDelay, cancellationTokenSource.Token).ContinueWith(_ => callback());
        }
    }
}
