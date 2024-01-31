using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Services;

namespace Raid.Toolkit.Extensibility.Host
{
    public class ExtensionHost : IExtensionManagement
    {
        public ExtensionBundle Bundle { get; }
        private readonly IServiceProvider ServiceProvider;
        private readonly IMenuManager MenuManager;
        private readonly IAccountManager AccountManager;
        private readonly IWindowManager WindowManager;
        private readonly IScopedServiceManager ScopedServices;
        private readonly IServiceManager ServiceManager;
        private readonly IPackageLoader Loader;
        private readonly ILogger<ExtensionHost> Logger;
        private readonly Dictionary<Type, IDisposable> Instances = new();
        private IExtensionPackage? ExtensionPackage;
        public ExtensionState State { get; private set; }

        public ExtensionHost(
            // args
            ExtensionBundle bundle,
            // injected
            IScopedServiceManager scopedServices,
            IServiceManager serviceManager,
            IServiceProvider serviceProvider,
            IAccountManager accountManager,
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
            ServiceManager = serviceManager;
            ServiceProvider = serviceProvider;
            AccountManager = accountManager;
            MenuManager = menuManager;
            WindowManager = windowManager;
            Logger = logger;
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

        public async Task Load()
        {
            if (State != ExtensionState.None || ExtensionPackage != null)
            {
                Logger.LogError("The extension {ExtensionId} is in invalid state {State}", Bundle.Id, State);
                return;
            }

            try
            {
                ExtensionPackage = await Loader.LoadPackage(Bundle);
                State = ExtensionState.Loaded;
            }
            catch (TypeLoadException ex)
            {
                State = ExtensionState.Error;
                Logger.LogError(ExtensionError.TypeLoadFailure.EventId(), ex, "Failed to load extension");
                throw new FileLoadException("Failed to load extension");
            }
            catch (Exception ex)
            {
                Logger.LogError(ExtensionError.FailedToLoad.EventId(), ex, "Failed to load extension");
                throw new FileLoadException("Failed to load extension");
            }
        }

        public void Activate()
        {
            try
            {
                if (State == ExtensionState.Activated)
                    return;

                if (ExtensionPackage == null)
                    throw new InvalidOperationException("ExtensionPackage is not set");

                ExtensionPackage.OnActivate(this);
                State = ExtensionState.Activated;
            }
            catch (Exception ex)
            {
                Logger.LogError(ExtensionError.FailedToActivate.EventId(), ex, "Failed to load extension");
                State = ExtensionState.Error;
            }
        }

        public void Deactivate()
        {
            if (State is not ExtensionState.Activated and not ExtensionState.Error)
                return;

            if (ExtensionPackage == null)
                throw new InvalidOperationException("ExtensionPackage is not set");

            ExtensionPackage.OnDeactivate(this);
            State = ExtensionState.Disabled;
        }

        public void Install()
        {
            if (ExtensionPackage == null)
                throw new InvalidOperationException("ExtensionPackage is not set");

            ExtensionPackage.OnInstall(this);
        }

        public void Uninstall()
        {
            try
            {
                if (ExtensionPackage == null)
                    throw new InvalidOperationException("ExtensionPackage is not set");

                ExtensionPackage.OnUninstall(this);
                State = ExtensionState.PendingUninstall;
            }
            catch (Exception ex)
            {
                Logger.LogError(ExtensionError.FailedToActivate.EventId(), ex, "Failed to uninstall extension");
                State = ExtensionState.Error;
            }
        }

        public void ShowUI()
        {
            if (!CanShowUI)
                throw new ApplicationException("Cannot show user interface");

            if (ExtensionPackage == null)
                throw new InvalidOperationException("ExtensionPackage is not set");

            ExtensionPackage.ShowUI();
        }

        public bool CanShowUI => WindowManager.CanShowUI;

        public int HandleRequest(Uri requestUri)
        {
            try
            {
                return ExtensionPackage?.HandleRequest(requestUri) == true ? 1 : 0;
            }
            catch (Exception ex)
            {
                Logger.LogError(ExtensionError.HandleRequestFailed.EventId(), ex, "Failed to uninstall extension");
                return -2;
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

        public IExtensionStorage GetStorage(IAccount account, bool enableCache)
        {
            IDataStorage storage = enableCache
                ? ServiceProvider.GetRequiredService<CachedDataStorage<PersistedDataStorage>>()
                : ServiceProvider.GetRequiredService<PersistedDataStorage>();

            return new ExtensionStorage(storage, new(account.Id, Bundle.Id));
        }

        public T CreateInstance<T>(params object[] args)
        {
            IServiceProvider scope = ServiceProvider.CreateScope().ServiceProvider;
            T instance = ActivatorUtilities.CreateInstance<T>(scope, args);
            return instance;
        }


        public IDisposable RegisterWindow<T>(WindowOptions options) where T : class
        {
            WindowManager.RegisterWindow<T>(options);
            return new HostResourceHandle(() => WindowManager.UnregisterWindow<T>());
        }

        public IWindowAdapter<T> CreateWindow<T>() where T : class
        {
            return WindowManager.CreateWindow<T>();
        }

        public IDisposable RegisterMessageScopeHandler<T>(T handler) where T : IMessageScopeHandler
        {
            ScopedServices.AddMessageScopeHandler(handler);
            return new HostResourceHandle(() => ScopedServices.RemoveMessageScopeHandler(handler));
        }

        [Obsolete("Use RegisterMessageScopeHandler<T>(T handler)")]
        public IDisposable RegisterMessageScopeHandler<T>() where T : IMessageScopeHandler
        {
            IServiceProvider scope = ServiceProvider.CreateScope().ServiceProvider;
            T instance = ActivatorUtilities.CreateInstance<T>(scope);
            return RegisterMessageScopeHandler(instance);
        }

        public IDisposable RegisterBackgroundService<T>(T service) where T : IBackgroundService
        {
            return ServiceManager.AddService(service);
        }

        [Obsolete("Use RegisterBackgroundService<T>(T service)")]
        public IDisposable RegisterBackgroundService<T>() where T : IBackgroundService
        {
            IServiceProvider scope = ServiceProvider.CreateScope().ServiceProvider;
            T instance = ActivatorUtilities.CreateInstance<T>(scope);
            return RegisterBackgroundService(instance);
        }

        public IDisposable RegisterAccountExtension<T>(T factory) where T : IAccountExtensionFactory
        {
            AccountManager.RegisterAccountExtension(Bundle.Manifest, factory);
            return new HostResourceHandle(() => AccountManager.UnregisterAccountExtension(Bundle.Manifest, factory));
        }

        public IEnumerable<IAccount> GetAccounts()
        {
            return AccountManager.Accounts;
        }

        public bool TryGetAccount(string accountId, [NotNullWhen(true)] out IAccount? account)
        {
            return AccountManager.TryGetAccount(accountId, out account);
        }


        public IDisposable RegisterMenuEntry(IMenuEntry entry)
        {
            MenuManager.AddEntry(entry);
            return new HostResourceHandle(() => MenuManager.RemoveEntry(entry));
        }
        #endregion
    }
}
