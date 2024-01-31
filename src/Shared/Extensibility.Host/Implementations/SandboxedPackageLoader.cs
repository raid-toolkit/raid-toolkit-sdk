using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace Raid.Toolkit.Extensibility.Host;

public class SandboxedPackageLoader : IPackageLoader, IDisposable
{
	private readonly ConcurrentDictionary<string, ExtensionSandbox> LoadedPackages = new();
	private readonly IServiceProvider ServiceProvider;
	private bool IsDisposed;

	public SandboxedPackageLoader(IServiceProvider serviceProvider)
	{
		ServiceProvider = serviceProvider;
	}

	public async Task<IExtensionPackage> LoadPackage(ExtensionBundle bundle)
	{
		IExtensionPackage package = LoadedPackages.GetOrAdd(
			bundle.Manifest.Id,
			(id) => ActivatorUtilities.CreateInstance<ExtensionSandbox>(ServiceProvider, bundle)
		);
		await package.Load();
		return package;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!IsDisposed)
		{
			if (disposing)
			{
				foreach (var package in LoadedPackages.Values)
					package.Dispose();

				LoadedPackages.Clear();
			}

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
