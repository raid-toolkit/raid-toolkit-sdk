using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Raid.Toolkit.Extensibility.Host
{
    public class ExtensionSandbox : IDisposable, IExtensionPackage
    {
        private bool IsDisposed;
        private readonly ExtensionBundle Bundle;
        private readonly IPackageInstanceFactory InstanceFactory;
        private ExtensionLoadContext LoadContext;
        private IExtensionPackage Instance;
        private readonly ILogger<ExtensionSandbox> Logger;

        public ExtensionManifest Manifest => Bundle.Manifest;
        public Assembly ExtensionAsm { get; private set; }

        [ActivatorUtilitiesConstructor]
        public ExtensionSandbox(
            IPackageInstanceFactory instanceFactory,
            ILogger<ExtensionSandbox> logger,
            ExtensionBundle bundle)
        {
            Logger = logger;
            InstanceFactory = instanceFactory;
            Bundle = bundle;
            if (bundle.Assembly != null)
            {
                ExtensionAsm = bundle.Assembly;
            }
            else
            {
                LoadContext = new(bundle.Location);
                ExtensionAsm = LoadContext.LoadFromAssemblyPath(bundle.GetExtensionEntrypointDll());
            }
        }

        private Type GetPackageType()
        {
            Type packageType = ExtensionAsm.GetType(Manifest.Type);
            return packageType ?? throw new EntryPointNotFoundException($"Could not load type {Manifest.Type} from the package");
        }

        public void Load()
        {
            Logger.LogInformation($"Loading extension {Manifest.Id}");
            if (ExtensionAsm.ReflectionOnly)
            {
                ExtensionAsm = Assembly.LoadFrom(Bundle.GetExtensionEntrypointDll());
            }
            _ = EnsureInstance();
        }

        private IExtensionPackage EnsureInstance()
        {
            return Instance ??= InstanceFactory.CreateInstance(GetPackageType());
        }

        public void OnActivate(IExtensionHost host)
        {
            Logger.LogInformation($"Activating extension {Manifest.Id}");
            _ = EnsureInstance();
            try
            {
                Instance.OnActivate(host);
            }
            catch (Exception e)
            {
                Logger.LogError($"Activation error {Manifest.Id}", e);
                OnDeactivate(host);
                Dispose();
            }
        }

        public void OnDeactivate(IExtensionHost host)
        {
            if (IsDisposed)
                return;
            Logger.LogInformation($"Deactivating extension {Manifest.Id}");
            try
            {
                Instance?.OnDeactivate(host);
            }
            catch (Exception e)
            {
                Logger.LogError($"Deactivation error {Manifest.Id}", e);
            }
        }

        public void OnInstall(IExtensionHost host)
        {
            if (IsDisposed)
                return;
            Logger.LogInformation($"Installing extension {Manifest.Id}");
            EnsureInstance().OnInstall(host);
        }

        public void OnUninstall(IExtensionHost host)
        {
            if (IsDisposed)
                return;
            Logger.LogInformation($"Uninstalling extension {Manifest.Id}");
            EnsureInstance().OnUninstall(host);
        }

        public void ShowUI()
        {
            if (IsDisposed)
                return;
            Logger.LogInformation($"Showing extension UI {Manifest.Id}");
            Instance?.ShowUI();
        }

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            if (disposing)
            {
                Logger.LogInformation($"Disposing extension {Manifest.Id}");
                Instance?.Dispose();
                LoadContext?.Unload();
            }

            Instance = null;
            ExtensionAsm = null;
            LoadContext = null;
            IsDisposed = true;
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
