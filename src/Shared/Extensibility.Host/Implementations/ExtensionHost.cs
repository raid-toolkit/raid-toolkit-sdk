using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Providers;
using Raid.Toolkit.Extensibility.Services;

namespace Raid.Toolkit.Extensibility.Host
{
    public class ExtensionHost : IExtensionHost
    {
        private readonly IExtensionPackage ExtensionPackage;
        private readonly ExtensionBundle Bundle;
        private readonly IServiceProvider ServiceProvider;
        private readonly IMenuManager MenuManager;
        private readonly IWindowManager WindowManager;
        private readonly IScopedServiceManager ScopedServices;
        private readonly IServiceManager ServiceManager;
        private readonly IContextDataManager DataManager;
        private readonly Dictionary<Type, IDisposable> Instances = new();

        public ExtensionHost(
            // args
            IExtensionPackage package,
            ExtensionBundle bundle,
            // injected
            IScopedServiceManager scopedServices,
            IContextDataManager dataManager,
            IServiceManager serviceManager,
            IServiceProvider serviceProvider,
            IMenuManager menuManager,
            IWindowManager windowManager
            )
        {
            ExtensionPackage = package;
            Bundle = bundle;

            ScopedServices = scopedServices;
            DataManager = dataManager;
            ServiceManager = serviceManager;
            ServiceProvider = serviceProvider;
            MenuManager = menuManager;
            WindowManager = windowManager;
        }

        public static ExtensionHost CreateHost(IServiceProvider serviceProvider, IExtensionPackage package, ExtensionBundle bundle)
        {
            return ActivatorUtilities.CreateInstance<ExtensionHost>(serviceProvider, package, bundle);
        }

        public Regex[] GetIncludeTypes()
        {
            if (Bundle.Manifest.Codegen == null)
                return Array.Empty<Regex>();

            Regex[] typePatterns = Bundle.Manifest.Codegen.Types
                .Select(pattern => new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled))
                .ToArray();
            return typePatterns;
        }

        public void Activate()
        {
            ExtensionPackage.OnActivate(this);
        }

        public void Deactivate()
        {
            ExtensionPackage.OnDeactivate(this);
        }

        public void Install()
        {
            ExtensionPackage.OnInstall(this);
        }

        public void Uninstall()
        {
            ExtensionPackage.OnUninstall(this);
        }

        public void ShowUI()
        {
            ExtensionPackage.ShowUI();
        }

        #region IExtensionHost
        public IExtensionStorage GetStorage(bool enableCache)
        {
            IDataStorage storage = enableCache
                ? ServiceProvider.GetRequiredService<CachedDataStorage<PersistedDataStorage>>()
                : ServiceProvider.GetRequiredService<PersistedDataStorage>();

            return new ExtensionStorage(storage, new(Bundle.Id));
        }


        public T CreateInstance<T>(params object[] args) where T : IDisposable
        {
            IServiceProvider scope = ServiceProvider.CreateScope().ServiceProvider;
            T instance = ActivatorUtilities.CreateInstance<T>(scope, args);
            _ = Instances.TryAdd(typeof(T), instance);
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
    }
}
