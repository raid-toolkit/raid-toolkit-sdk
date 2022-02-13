using System;
using System.Collections.Concurrent;

namespace Raid.Toolkit.Extensibility
{
    public class SandboxedPackageLoader : IPackageLoader, IDisposable
    {
        private readonly ConcurrentDictionary<string, IExtensionPackage> LoadedPackages = new();
        private bool IsDisposed;

        private IPackageInstanceFactory InstanceFactory;
        public SandboxedPackageLoader(IPackageInstanceFactory instanceFactory) => InstanceFactory = instanceFactory;

        public IExtensionPackage LoadPackage(PackageDescriptor descriptor)
        {
            return LoadedPackages.GetOrAdd(descriptor.Id, (id) => new ExtensionSandbox(descriptor, InstanceFactory));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    foreach(var package in LoadedPackages.Values)
                        package.Dispose();

                    LoadedPackages.Clear();
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
