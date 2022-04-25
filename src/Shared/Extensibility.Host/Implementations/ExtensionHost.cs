using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Extensibility.Providers;
using Raid.Toolkit.Extensibility.Services;

namespace Raid.Toolkit.Extensibility.Host
{
    public class ExtensionHost : IExtensionHostController, IDisposable
    {
        private readonly IModelLoader ModelLoader;
        private readonly IPackageLoader PackageLoader;
        private readonly IPackageManager Locator;
        private readonly IServiceProvider ServiceProvider;
        private readonly IMenuManager MenuManager;
        private readonly IWindowManager WindowManager;
        private readonly IScopedServiceManager ScopedServices;
        private readonly IServiceManager ServiceManager;
        private readonly IContextDataManager DataManager;
        private readonly Dictionary<string, IExtensionPackage> ExtensionPackages = new();
        private readonly Dictionary<Type, IDisposable> Instances = new();
        private bool IsDisposed;

        public ExtensionHost(
            IPackageManager locator,
            IPackageLoader loader,
            IScopedServiceManager scopedServices,
            IContextDataManager dataManager,
            IModelLoader modelLoader,
            IServiceManager serviceManager,
            IServiceProvider serviceProvider,
            IMenuManager menuManager,
            IWindowManager windowManager
            )
        {
            Locator = locator;
            PackageLoader = loader;
            ScopedServices = scopedServices;
            DataManager = dataManager;
            ModelLoader = modelLoader;
            ServiceManager = serviceManager;
            ServiceProvider = serviceProvider;
            MenuManager = menuManager;
            WindowManager = windowManager;
        }


        #region IExtensionHost

        public T CreateInstance<T>(params object[] args) where T : IDisposable
        {
            IServiceProvider scope = ServiceProvider.CreateScope().ServiceProvider;
            T instance = ActivatorUtilities.CreateInstance<T>(scope, args);
            Instances.TryAdd(typeof(T), instance);
            return instance;
        }


        public IDisposable RegisterWindow<T>(WindowOptions options) where T : Form
        {
            WindowManager.RegisterWindow<T>(options);
            return new HostResourceHandle(() => WindowManager.UnregisterWindow<T>());
        }

        public T CreateWindow<T>() where T : Form
        {
            return WindowManager.CreateWindow<T>();
        }


        [Obsolete]
        public T GetInstance<T>() where T : IDisposable
        {
            return (T)Instances[typeof(T)];
        }


        public IDisposable RegisterMessageScopeHandler<T>(T handler) where T : IMessageScopeHandler
        {
            ScopedServices.AddMessageScopeHandler(handler);
            return new HostResourceHandle(() => ScopedServices.RemoveMessageScopeHandler(handler));
        }

        [Obsolete]
        public IDisposable RegisterMessageScopeHandler<T>() where T : IMessageScopeHandler
        {
            IServiceProvider scope = ServiceProvider.CreateScope().ServiceProvider;
            T instance = ActivatorUtilities.CreateInstance<T>(scope);
            return RegisterMessageScopeHandler(instance);
        }

        public IDisposable RegisterDataProvider<T>(T provider) where T : IDataProvider
        {
            IServiceProvider scope = ServiceProvider.CreateScope().ServiceProvider;
            T instance = ActivatorUtilities.CreateInstance<T>(scope);
            return DataManager.AddProvider(instance);
        }

        [Obsolete]
        public IDisposable RegisterDataProvider<T>() where T : IDataProvider
        {
            return DataManager.AddProvider<T>();
        }

        public IDisposable RegisterBackgroundService<T>(T service) where T : IBackgroundService
        {
            return ServiceManager.AddService(service);
        }

        [Obsolete]
        public IDisposable RegisterBackgroundService<T>() where T : IBackgroundService
        {
            IServiceProvider scope = ServiceProvider.CreateScope().ServiceProvider;
            T instance = ActivatorUtilities.CreateInstance<T>(scope);
            return RegisterBackgroundService(instance);
        }

        public IDisposable RegisterMenuEntry(IMenuEntry entry)
        {
            MenuManager.AddEntry(entry);
            return new HostResourceHandle(() => MenuManager.RemoveEntry(entry));
        }
        #endregion

        #region IExtensionHostController
        public async Task LoadExtensions()
        {
            foreach (var pkg in Locator.GetAllPackages())
                ExtensionPackages.Add(pkg.Id, PackageLoader.LoadPackage(pkg));

            var typePatterns = ExtensionPackages.Values.OfType<IRequireCodegen>().SelectMany(cg => cg.TypeFilter.IncludeTypes);
            await Task.Run(() => ModelLoader.BuildAndLoad(typePatterns, false));
        }

        public void ActivateExtensions()
        {
            foreach (var pkg in ExtensionPackages.Values)
                pkg.OnActivate(this);
        }

        public void ShowExtensionUI()
        {
            foreach (var pkg in ExtensionPackages.Values)
                pkg.ShowUI();
        }

        public void DeactivateExtensions()
        {
            foreach (var pkg in ExtensionPackages.Values)
                pkg.OnDeactivate(this);

            ExtensionPackages.Clear();
            Instances.Clear();
        }

        public void InstallPackage(ExtensionBundle pkgToInstall, bool activate)
        {
            ExtensionBundle installedPkg = Locator.AddPackage(pkgToInstall);
            var pkg = PackageLoader.LoadPackage(installedPkg);
            pkg.OnInstall(this);
            ExtensionPackages.Add(installedPkg.Id, pkg);
        }

        public void UninstallPackage(string id)
        {
            if (ExtensionPackages.Remove(id, out var pkg))
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
