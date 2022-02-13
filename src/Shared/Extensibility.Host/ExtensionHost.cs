using System;
using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility
{
    public class ExtensionHost : IExtensionHost, IDisposable
    {
        private readonly IPackageLocator Locator;
        private readonly IPackageLoader Loader;
        private readonly Dictionary<string, IExtensionPackage> ExtensionPackages = new();
        private bool IsDisposed;

        public ExtensionHost(IPackageLocator locator, IPackageLoader loader) => (Locator, Loader) = (locator, loader);

        public void LoadExtensions()
        {
            foreach (var pkg in Locator.GetAllPackages())
            {
                ExtensionPackages.Add(pkg.Id, Loader.LoadPackage(pkg));
            }
        }

        public void ActivateExtensions()
        {
            foreach (var (_, pkg) in ExtensionPackages)
            {
                pkg.OnActivate(this);
            }
        }

        public void InstallPackage(PackageDescriptor descriptor, bool activate)
        {
            var pkg = Loader.LoadPackage(descriptor);
            pkg.OnInstall(this);
            ExtensionPackages.Add(descriptor.Id, pkg);
        }

        public void UninstallPackage(PackageDescriptor descriptor)
        {
            if (ExtensionPackages.Remove(descriptor.Id, out var pkg))
            {
                pkg.OnDeactivate(this);
                pkg.OnUninstall(this);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    foreach (var pkg in ExtensionPackages.Values)
                    {
                        pkg.OnDeactivate(this);
                    }
                    ExtensionPackages.Clear();
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
