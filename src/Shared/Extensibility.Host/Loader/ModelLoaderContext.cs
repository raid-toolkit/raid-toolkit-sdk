using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Raid.Toolkit.Extensibility.Host
{
    public class ModelLoaderContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver Resolver;

        public ModelLoaderContext(string pluginPath) : base(isCollectible: true)
        {
            Resolver = new AssemblyDependencyResolver(Path.GetDirectoryName(pluginPath));
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = Resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = Resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
