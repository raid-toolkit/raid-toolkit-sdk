using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Raid.Toolkit.Extensibility
{
    internal class ExtensionSandbox : MarshalByRefObject, IDisposable
    {
        private string DllPath;
        private bool IsDisposed;
        private ExtensionLoadContext LoadContext;
        private Assembly ExtensionAsm;

        internal ExtensionSandbox(string dllPath)
        {
            DllPath = dllPath;
            LoadContext = new(dllPath);
            ExtensionAsm = LoadContext.LoadFromAssemblyPath(dllPath);
        }

        public static ExtensionSandbox Create(string dllPath)
        {
            string appDomainName = $"sandbox:{Path.GetFileNameWithoutExtension(dllPath)}";
            AppDomain appDomain = AppDomain.CreateDomain(appDomainName);
            var sandbox = (ExtensionSandbox)appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(ExtensionSandbox).FullName, new object[] { dllPath });
            return sandbox;
        }

        public PackageDescriptor QueryDescriptor()
        {
            Type packageType = GetPackageType();
            return packageType.GetCustomAttribute<ExtensionPackageAttribute>().Descriptor;
        }

        private Type GetPackageType()
        {
            return ExtensionAsm.ExportedTypes.SingleOrDefault(t => t.GetInterfaces().Contains(typeof(IExtensionPackage)));
        }

        public IExtensionPackage Load()
        {
            if (ExtensionAsm.ReflectionOnly)
            {
                ExtensionAsm = Assembly.LoadFrom(DllPath);
            }
            return (IExtensionPackage)Activator.CreateInstance(GetPackageType());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    LoadContext.Unload();
                }

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
    }
}
