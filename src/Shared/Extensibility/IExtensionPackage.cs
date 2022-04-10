using Raid.Toolkit.Common;
using System;

namespace Raid.Toolkit.Extensibility
{
    public interface IExtensionPackage : IDisposable
    {
        public void OnActivate(IExtensionHost host);
        public void OnDeactivate(IExtensionHost host);
        public void OnInstall(IExtensionHost host);
        public void OnUninstall(IExtensionHost host);
        public void ShowUI();
    }

    public abstract class ExtensionPackage : IExtensionPackage
    {
        protected readonly DisposableCollection Disposables = new();
        protected bool IsDisposed;

        public abstract void OnActivate(IExtensionHost host);
        public virtual void OnDeactivate(IExtensionHost host)
        {
            Disposables.Reset();
        }
        public virtual void OnInstall(IExtensionHost host) { }
        public virtual void OnUninstall(IExtensionHost host) { }
        public virtual void ShowUI() { }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposables.Dispose();
            }
            IsDisposed = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
