using System;
using System.Threading;

namespace Raid.Toolkit.Common
{
    public static class ActivationHelper
    {
        public static readonly string MutexName = "RaidToolkit Singleton";
        public static bool IsRaidToolkitRunning()
        {
            using var mutex = new Mutex(false, MutexName);
            bool isRunning = !mutex.WaitOne(TimeSpan.Zero);
            if (!isRunning)
            {
                mutex.ReleaseMutex();
            }
            return isRunning;
        }
    }
}