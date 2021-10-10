using System;
using System.Threading.Tasks;

namespace Raid.Service
{
    public static class TaskExtensions
    {
        public static Task RunAfter(int millisecondsDelay, Action callback)
        {
            return Task.Delay(millisecondsDelay).ContinueWith(_ => callback());
        }
    }
}