using Microsoft.Extensions.Logging;
using Raid.Toolkit.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host
{
    public class ExtensionHostController : IExtensionHostController, IDisposable
    {
        private readonly IModelLoader ModelLoader;
        private readonly IPackageLoader PackageLoader;
        private readonly IPackageManager PackageManager;
        private readonly IServiceProvider ServiceProvider;
        private readonly IWindowManager WindowManager;
        private readonly ILogger<ExtensionHostController> Logger;
        private readonly Dictionary<string, ExtensionHost> ExtensionPackages = new();
        private readonly Dictionary<Type, IDisposable> Instances = new();
        private bool IsDisposed;

        public ExtensionHostController(
            IPackageManager locator,
            IPackageLoader loader,
            IModelLoader modelLoader,
            IServiceProvider serviceProvider,
            IWindowManager windowManager,
            ILogger<ExtensionHostController> logger
            )
        {
            PackageManager = locator;
            PackageLoader = loader;
            ModelLoader = modelLoader;
            ServiceProvider = serviceProvider;
            WindowManager = windowManager;
            Logger = logger;
        }

        #region IExtensionHostController
        public IReadOnlyList<IExtensionManagement> GetExtensions()
        {
            return ExtensionPackages.Values.ToList();
        }

        public Task LoadExtensions()
        {
            foreach (var pkg in PackageManager.GetAllPackages())
            {
                try
                {
                    ExtensionHost extensionHost = ExtensionHost.CreateHost(ServiceProvider, pkg);
                    ExtensionPackages.Add(pkg.Id, extensionHost);
                }
                catch (TypeLoadException ex)
                {
                    Logger.LogError(ExtensionError.TypeLoadFailure.EventId(), ex, "Failed to load extension");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ExtensionError.FailedToLoad.EventId(), ex, "Failed to load extension");
                }
            }

            var typePatterns = ExtensionPackages.Values.SelectMany(host => host.GetIncludeTypes());
            return Task.Run(() => ModelLoader.BuildAndLoad(typePatterns, true));
        }

        public void ActivateExtensions()
        {
            foreach (var pkg in ExtensionPackages.Values)
                pkg.Activate();
        }

        public void ShowExtensionUI()
        {
            foreach (var pkg in ExtensionPackages.Values)
                pkg.ShowUI();

            WindowManager.RestoreWindows();
        }

        public void DeactivateExtensions()
        {
            foreach (var pkg in ExtensionPackages.Values)
                pkg.Deactivate();

            ExtensionPackages.Clear();
            Instances.Clear();
        }

        public void InstallPackage(ExtensionBundle descriptor, bool activate)
        {
            ExtensionBundle installedPkg = PackageManager.AddPackage(descriptor);
            ExtensionHost host = ExtensionHost.CreateHost(ServiceProvider, installedPkg);
            host.Install();
            ExtensionPackages.Add(installedPkg.Id, host);
        }

        public void EnablePackage(string packageId)
        {
            if (ExtensionPackages.Remove(packageId, out var pkg))
            {
                pkg.Activate();
                // TODO: Persist that it should be enabled
            }
        }

        public void DisablePackage(string packageId)
        {
            if (ExtensionPackages.Remove(packageId, out var pkg))
            {
                pkg.Deactivate();
                // TODO: Persist that it should be disabled
            }
        }

        public void UninstallPackage(string packageId)
        {
            if (ExtensionPackages.Remove(packageId, out var pkg))
            {
                pkg.Deactivate();
                pkg.Uninstall();
                PackageManager.RemovePackage(packageId);
            }
        }

        public ExtensionHost GetExtensionPackageHost(string packageId)
        {
            return ExtensionPackages[packageId];
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
                    DeactivateExtensions();
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
