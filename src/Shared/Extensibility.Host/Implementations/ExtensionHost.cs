using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Extensibility.HostInterfaces;
using Raid.Toolkit.Extensibility.Providers;
using Raid.Toolkit.Extensibility.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host
{
	public class ExtensionHost : IExtensionHostController, IDisposable
    {
        private readonly IPackageManager Locator;
        private readonly IPackageLoader Loader;
        private readonly IScopedServiceManager ScopedServices;
        private readonly IContextDataManager DataManager;
        private readonly Dictionary<string, IExtensionPackage> ExtensionPackages = new();
        private bool IsDisposed;

        public ExtensionHost(IPackageManager locator, IPackageLoader loader, IScopedServiceManager scopedServices, IContextDataManager dataManager) =>
            (Locator, Loader, ScopedServices, DataManager) = (locator, loader, scopedServices, dataManager);

        #region IExtensionHost
        public IDisposable RegisterMessageScopeHandler(IMessageScopeHandler handler)
        {
            ScopedServices.AddMessageScopeHandler(handler);
            return new HostResourceHandle(() => ScopedServices.RemoveMessageScopeHandler(handler));
        }

        public IDisposable RegisterDataProvider<T>() where T : IDataProvider
        {
            return DataManager.AddProvider<T>();
        }
        #endregion

        #region IExtensionHostController
        public async Task LoadExtensions()
        {
            foreach (var pkg in Locator.GetAllPackages())
                ExtensionPackages.Add(pkg.Id, Loader.LoadPackage(pkg));

            var typePatterns = ExtensionPackages.Values.OfType<IRequireCodegen>().SelectMany(cg => cg.TypePatterns);
            ModelLoader loader = new(typePatterns);
            await Task.Run(() => loader.Load(false));
        }

        public void ActivateExtensions()
        {
            foreach (var pkg in ExtensionPackages.Values)
                pkg.OnActivate(this);
        }

        public void InstallPackage(PackageDescriptor pkgToInstall, bool activate)
        {
            PackageDescriptor installedPkg = Locator.AddPackage(pkgToInstall);
            var pkg = Loader.LoadPackage(installedPkg);
            pkg.OnInstall(this);
            ExtensionPackages.Add(installedPkg.Id, pkg);
        }

        public void UninstallPackage(PackageDescriptor descriptor)
        {
            if (ExtensionPackages.Remove(descriptor.Id, out var pkg))
            {
                pkg.OnDeactivate(this);
                pkg.OnUninstall(this);
            }
        }
        #endregion

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    foreach (var pkg in ExtensionPackages.Values)
                        pkg.OnDeactivate(this);
                    
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
		#endregion
	}
}
