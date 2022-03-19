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

        public Assembly ExtensionAsm { get; private set; }
        public CodegenTypeFilter TypeFilter
        {
            get
            {
                List<Regex> typePatterns = new();
                JsonSerializer serializer = new();
                using (var stream = ExtensionAsm.GetManifestResourceStream("PackageManifest"))
                using (StreamReader reader = new(stream))
                using (JsonTextReader textReader = new(reader))
                {
                    ExtensionManifest manifest = serializer.Deserialize<ExtensionManifest>(textReader);
                    if (manifest.Codegen?.Types != null)
                    {
                        typePatterns.AddRange(manifest.Codegen.Types.Select(pattern => new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled)));
                    }
                }
                return new(typePatterns.ToArray());
            }
        }

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
        }

        private Type GetPackageType()
        {
            // TEMP: ID MUST match the DLL filename
            // TODO: Verify the attribute sets the same ID as the DLL name
            try
            {
                return ExtensionAsm.ExportedTypes.Single(t => t.GetInterfaces().Contains(typeof(IExtensionPackage)));
            }
            catch
            {
                throw new ApplicationException($"Extension DLLs must contain exactly one class that implements IExtensionPackage.");
            }
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
            return Instance = InstanceFactory.CreateInstance(GetPackageType(), Descriptor);
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
