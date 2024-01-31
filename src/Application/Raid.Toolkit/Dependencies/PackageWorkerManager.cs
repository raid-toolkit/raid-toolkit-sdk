using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Raid.Toolkit;

public class PackageWorkerManager : IPackageWorkerManager, IDisposable
{
	private readonly IPackageManager PackageManager;
	private readonly IServiceProvider ServiceProvider;
	private readonly ILogger<IPackageWorkerManager> Logger;
	private readonly Dictionary<string, IManagedPackageWorker> ExtensionWorkers = new();
	private bool IsDisposed;

	public PackageWorkerManager(
		IPackageManager locator,
		IServiceProvider serviceProvider,
		ILogger<PackageWorkerManager> logger
		)
	{
		PackageManager = locator;
		ServiceProvider = serviceProvider;
		Logger = logger;
	}

	#region IExtensionHostController

	public bool TryGetPackageWorker(string packageId, [NotNullWhen(true)] out IManagedPackageWorker? worker)
	{
		if (ExtensionWorkers.TryGetValue(packageId, out worker))
		{
			return true;
		}
		try
		{
			ExtensionBundle pkg = PackageManager.GetPackage(packageId);
			worker = ActivatorUtilities.CreateInstance<ManagedPackageWorker>(ServiceProvider, pkg);
			ExtensionWorkers.Add(pkg.Id, worker);
			return true;
		}
		catch (Exception ex)
		{
			Logger.LogError(ExtensionError.FailedToLoad.EventId(), ex, "Failed to load extension from manifest");
		}
		return false;
	}

	public Task StartExtensions()
	{
		foreach (ExtensionBundle bundle in PackageManager.GetAllPackages())
		{
			if (!TryGetPackageWorker(bundle.Id, out IManagedPackageWorker? worker))
				continue; // error occurred, logged above
			worker.Start();
		}
		return Task.CompletedTask;
	}

	public async Task StopExtensions()
	{
		await Task.WhenAll(ExtensionWorkers.Values.Select(worker => worker.Stop()));
	}

	public void DisablePackage(string packageId)
	{
	}

	public void EnablePackage(string packageId)
	{
	}


	#endregion

	#region IDisposable
	protected virtual void Dispose(bool disposing)
	{
		if (!IsDisposed)
		{
			if (disposing)
			{
				// dispose managed state (managed objects)
				Task.WaitAll(StopExtensions());
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
	#endregion
}
