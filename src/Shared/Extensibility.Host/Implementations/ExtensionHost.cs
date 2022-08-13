using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Providers;
using Raid.Toolkit.Extensibility.Services;

namespace Raid.Toolkit.Extensibility.Host
{
    public class ExtensionHost : IExtensionManagement
    {
        private IExtensionPackage _ExtensionPackage;
        public ExtensionBundle Bundle { get; }
        private readonly IServiceProvider ServiceProvider;
        private readonly IMenuManager MenuManager;
        private readonly IWindowManager WindowManager;
        private readonly IScopedServiceManager ScopedServices;
        private readonly IServiceManager ServiceManager;
        private readonly IContextDataManager DataManager;
        private readonly IPackageLoader Loader;
        private readonly ILogger<ExtensionHost> Logger;
        private readonly Dictionary<Type, IDisposable> Instances = new();
        public ExtensionState State { get; private set; }

        public ExtensionHost(
            // args
            ExtensionBundle bundle,
            // injected
            IScopedServiceManager scopedServices,
            IContextDataManager dataManager,
            IServiceManager serviceManager,
            IServiceProvider serviceProvider,
            IMenuManager menuManager,
            IWindowManager windowManager,
            IPackageLoader loader,
            ILogger<ExtensionHost> logger
            )
        {
            State = ExtensionState.None;
            Loader = loader;
            Bundle = bundle;

            ScopedServices = scopedServices;
            DataManager = dataManager;
            ServiceManager = serviceManager;
            ServiceProvider = serviceProvider;
            MenuManager = menuManager;
            WindowManager = windowManager;
            Logger = logger;
        }

        private IExtensionPackage ExtensionPackage
        {
            get
            {
                if (State == ExtensionState.Error)
                    return null;

                if (_ExtensionPackage == null)
                {
                    try
                    {
                        _ExtensionPackage = Loader.LoadPackage(Bundle);
                        State = ExtensionState.Loaded;
                    }
                    catch (TypeLoadException ex)
                    {
                        State = ExtensionState.Error;
                        Logger.LogError(ExtensionError.TypeLoadFailure.EventId(), ex, "Failed to load extension");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ExtensionError.FailedToLoad.EventId(), ex, "Failed to load extension");
                    }
                }
                return _ExtensionPackage;
            }
        }

        public static ExtensionHost CreateHost(IServiceProvider serviceProvider, ExtensionBundle bundle)
        {
            return ActivatorUtilities.CreateInstance<ExtensionHost>(serviceProvider, bundle);
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
            try
            {
                if (State == ExtensionState.Activated)
                    return;

                ExtensionPackage.OnActivate(this);
                State = ExtensionState.Activated;
            }
            catch(Exception ex)
            {
                Logger.LogError(ExtensionError.FailedToActivate.EventId(), ex, "Failed to load extension");
                State = ExtensionState.Error;
            }
        }

        public void Deactivate()
        {
            if (State != ExtensionState.Activated && State != ExtensionState.Error)
                return;

            ExtensionPackage.OnDeactivate(this);
            State = ExtensionState.Disabled;
        }

        public void Install()
        {
            ExtensionPackage.OnInstall(this);
        }

        public void Uninstall()
        {
            try
            {
                ExtensionPackage.OnUninstall(this);
                State = ExtensionState.PendingUninstall;
            }
            catch(Exception ex)
            {
                Logger.LogError(ExtensionError.FailedToActivate.EventId(), ex, "Failed to uninstall extension");
                State = ExtensionState.Error;
            }
        }

        public void ShowUI()
        {
            if (!CanShowUI)
                throw new ApplicationException("Cannot show user interface");

            ExtensionPackage.ShowUI();
        }

        public bool CanShowUI => WindowManager.CanShowUI;

        public bool HandleRequest(Uri requestUri)
        {
            try
            {
                return ExtensionPackage.HandleRequest(requestUri);
            }
            catch(Exception ex)
            {
                Logger.LogError(ExtensionError.HandleRequestFailed.EventId(), ex, "Failed to uninstall extension");
                return false;
            }
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
