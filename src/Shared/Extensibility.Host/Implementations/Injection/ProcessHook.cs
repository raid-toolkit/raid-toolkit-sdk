using System;

namespace Raid.Toolkit.Injection
{
    internal class ProcessHook : IDisposable
    {
        private readonly int ProcessId;
        private bool disposedValue;

        public ProcessHook(int processId)
        {
            ProcessId = processId;
            AsyncHookThread.Current.Post(() => _ = Interop.InjectHook((uint)ProcessId));
        }

        public static void UnhookProcess()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                AsyncHookThread.Current.Post(() => _ = Interop.ReleaseHook((uint)ProcessId));
                disposedValue = true;
            }
        }

        ~ProcessHook()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
