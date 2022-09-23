using System.Runtime.InteropServices;

namespace Raid.Toolkit.Application.Core.Utility
{
    public static class TaskExtensions
    {
        public static void CoWait(this Task task)
        {
            IntPtr asyncEventHandle = CreateEvent(IntPtr.Zero, true, false, null);
            task.ContinueWith((t) =>
            {
                SetEvent(asyncEventHandle);
            });
            uint CWMO_DEFAULT = 0;
            uint INFINITE = 0xFFFFFFFF;
            _ = CoWaitForMultipleObjects(
               CWMO_DEFAULT, INFINITE, 1,
               new IntPtr[] { asyncEventHandle }, out uint handleIndex);
            if (task.IsFaulted)
                throw task.Exception ?? new AggregateException();
        }

        public static void CoWait<T>(this Task<T> task)
        {
            IntPtr asyncEventHandle = CreateEvent(IntPtr.Zero, true, false, null);
            task.ContinueWith((t) =>
            {
                SetEvent(asyncEventHandle);
            });
            uint CWMO_DEFAULT = 0;
            uint INFINITE = 0xFFFFFFFF;
            _ = CoWaitForMultipleObjects(
               CWMO_DEFAULT, INFINITE, 1,
               new IntPtr[] { asyncEventHandle }, out uint handleIndex);
            if (task.IsFaulted)
                throw task.Exception ?? new AggregateException();
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateEvent(
            IntPtr lpEventAttributes, bool bManualReset,
            bool bInitialState, string lpName);

        [DllImport("kernel32.dll")]
        private static extern bool SetEvent(IntPtr hEvent);

        [DllImport("ole32.dll")]
        private static extern uint CoWaitForMultipleObjects(
            uint dwFlags, uint dwMilliseconds, ulong nHandles,
            IntPtr[] pHandles, out uint dwIndex);
    }
}
