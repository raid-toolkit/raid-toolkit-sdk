using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Raid.Toolkit.Extensibility.Host;

public class ExtensionLoadContext : AssemblyLoadContext
{
	private readonly string[] SearchPaths;
	public ExtensionLoadContext(string pluginPath) : base(isCollectible: true)
	{
		SearchPaths = new string[] {
				Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? pluginPath,
				pluginPath
			}.Distinct().ToArray();
	}

	protected override Assembly? Load(AssemblyName assemblyName)
	{
		if (assemblyName.Name == "Raid.Interop")
			return null;

		Assembly? alreadyLoaded = Default.Assemblies.FirstOrDefault(asm => asm.GetName().Name == assemblyName.Name);
		if (alreadyLoaded != null)
			return alreadyLoaded;

		foreach (var searchPath in SearchPaths)
		{
			string testPath = Path.Combine(searchPath, $"{assemblyName.Name}.dll");
			if (File.Exists(testPath))
			{
				return Default.LoadFromAssemblyPath(testPath);
			}
		}

		return null;
	}

	protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
	{
		//foreach (var resolver in Resolvers)
		//{
		//    string libraryPath = resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
		//    if (libraryPath == null) continue;
		//    return LoadUnmanagedDllFromPath(libraryPath);
		//}

		return IntPtr.Zero;
	}
}
