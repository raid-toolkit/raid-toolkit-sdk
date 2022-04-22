using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Raid.Toolkit.Extensibility.Host
{
    public class ExtensionLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver[] Resolvers;

        public ExtensionLoadContext(string pluginPath) : base(isCollectible: true)
        {
            Resolvers = new AssemblyDependencyResolver[] {
                new AssemblyDependencyResolver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)),
                new AssemblyDependencyResolver(Path.GetDirectoryName(pluginPath))
            };
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            foreach(var resolver in Resolvers)
            {
                string assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);
                if (assemblyPath == null) continue;
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            foreach (var resolver in Resolvers)
            {
                string libraryPath = resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
                if (libraryPath == null) continue;
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
