using System;

namespace Raid.Toolkit.Extensibility
{
    public class HostResourceHandle : IDisposable
    {
        private readonly Action Callback;
        private bool IsDisposed;

        public HostResourceHandle(Action callback) => Callback = callback;

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    Callback();
                }
                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
