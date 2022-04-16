using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Raid.Toolkit.Extensibility.Host
{
    public class SandboxedPackageLoader : IPackageLoader, IDisposable
    {
        private readonly ConcurrentDictionary<string, IExtensionPackage> LoadedPackages = new();
        private bool IsDisposed;

        private IServiceProvider ServiceProvider;
        public SandboxedPackageLoader(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IExtensionPackage LoadPackage(ExtensionBundle bundle)
        {
            return LoadedPackages.GetOrAdd(
                bundle.Manifest.Id,
                (id) => ActivatorUtilities.CreateInstance<ExtensionSandbox>(ServiceProvider, bundle)
            );
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    foreach (var package in LoadedPackages.Values)
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
