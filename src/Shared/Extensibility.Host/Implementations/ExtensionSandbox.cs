using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Raid.Toolkit.Extensibility.Host
{
    internal class ExtensionSandbox : IDisposable, IRequireCodegen, IExtensionPackage
    {
        private bool IsDisposed;
        private readonly PackageDescriptor Descriptor;
        private readonly IPackageInstanceFactory InstanceFactory;
        private ExtensionLoadContext LoadContext;
        private Assembly ExtensionAsm;
        private IExtensionPackage Instance;

		public IEnumerable<Regex> TypePatterns
        {
            get
            {
                if (EnsureInstance() is IRequireCodegen requireCodegen)
                {
                    return requireCodegen.TypePatterns;
                }
                return Array.Empty<Regex>();
            }
        }

		internal ExtensionSandbox(PackageDescriptor descriptor, IPackageInstanceFactory instanceFactory)
        {
            Descriptor = descriptor;
            InstanceFactory = instanceFactory;
            LoadContext = new(descriptor.Location);
            ExtensionAsm = LoadContext.LoadFromAssemblyPath(descriptor.Location);
        }

        private Type GetPackageType()
        {
            return ExtensionAsm.ExportedTypes.Single(t => t.GetInterfaces().Contains(typeof(IExtensionPackage)));
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
