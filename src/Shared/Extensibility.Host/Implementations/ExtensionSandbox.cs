using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Raid.Toolkit.Extensibility.Host
{
    internal class ExtensionSandbox : IDisposable, IRequireCodegen, IExtensionPackage
    {
        private bool IsDisposed;
        private readonly PackageDescriptor Descriptor;
        private readonly IPackageInstanceFactory InstanceFactory;
        private ExtensionLoadContext LoadContext;
        private IExtensionPackage Instance;
        public readonly ExtensionManifest Manifest;

        public Assembly ExtensionAsm { get; private set; }
        public CodegenTypeFilter TypeFilter { get; }

        internal ExtensionSandbox(PackageDescriptor descriptor, IPackageInstanceFactory instanceFactory)
        {
            Descriptor = descriptor;
            InstanceFactory = instanceFactory;
            if (descriptor.Assembly != null)
            {
                ExtensionAsm = descriptor.Assembly;
            }
            else
            {
                LoadContext = new(descriptor.Location);
                ExtensionAsm = LoadContext.LoadFromAssemblyPath(descriptor.Location);
            }

            JsonSerializer serializer = new();
            using (var stream = ExtensionAsm.GetManifestResourceStream("PackageManifest"))
            using (StreamReader reader = new(stream))
            using (JsonTextReader textReader = new(reader))
            {
                Manifest = serializer.Deserialize<ExtensionManifest>(textReader);
            }

            Regex[] typePatterns = Manifest.Codegen.Types
                .Select(pattern => new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled))
                .ToArray();
            TypeFilter = new(typePatterns);
        }

        private Type GetPackageType()
        {
            Type packageType = ExtensionAsm.GetType(Manifest.Type);
            if (packageType == null)
            {
                throw new EntryPointNotFoundException($"Could not load type {Manifest.Type} from the package");
            }
            return packageType;
        }

        public void Load()
        {
            if (ExtensionAsm.ReflectionOnly)
            {
                ExtensionAsm = Assembly.LoadFrom(Descriptor.Location);
            }
            EnsureInstance();
        }

        private IExtensionPackage EnsureInstance()
        {
            return Instance ??= InstanceFactory.CreateInstance(GetPackageType(), Descriptor);
        }

        public void OnActivate(IExtensionHost host)
        {
            EnsureInstance();
            Instance.OnActivate(host);
        }

        public void OnDeactivate(IExtensionHost host)
        {
            Instance?.OnDeactivate(host);
        }

        public void OnInstall(IExtensionHost host)
        {
            EnsureInstance().OnInstall(host);
        }

        public void OnUninstall(IExtensionHost host)
        {
            EnsureInstance().OnUninstall(host);
        }

        public void ShowUI()
        {
            EnsureInstance().ShowUI();
        }

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    Instance?.Dispose();
                    LoadContext?.Unload();
                }

                Instance = null;
                ExtensionAsm = null;
                LoadContext = null;
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
