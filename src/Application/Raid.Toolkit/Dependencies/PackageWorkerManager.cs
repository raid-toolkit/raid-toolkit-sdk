using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Raid.Toolkit
{
	public class ManagedPackageWorker : IManagedPackageWorker, IDisposable
	{
		private Process? WorkerProcess;
		private readonly DependencySynthesizer Dependencies;
		private bool IsDisposed;
		private Job Job;

		public ManagedPackageWorker(ExtensionBundle bundle, IServiceProvider serviceProvider)
		{
			State = PackageState.None;
			Bundle = bundle;
			Dependencies = new(serviceProvider);
			Job = new();
		}

		private static ProcessStartInfo CreateWorkerStartInfo(string arguments)
		{
			if (RTKApplication.Current.Options.DebugBreak)
				arguments += " --dbg-break";
			return new(Path.Join(Path.GetDirectoryName(Environment.ProcessPath)!, "Raid.Toolkit.ExtensionHost.exe"), arguments);
		}

		public PackageState State { get; private set; }

		public ExtensionBundle Bundle { get; }

		public Process? StartProcess(ProcessStartInfo psi)
		{
			if (WorkerProcess?.HasExited == true)
				WorkerProcess = null;

			Process? actionProcess = Process.Start(psi);
			if (actionProcess != null)
				Job.AddProcess(actionProcess.SafeHandle);

			WorkerProcess ??= actionProcess;
			return WorkerProcess;
		}

		public async Task Disable()
		{
			await Stop();
			State = PackageState.Disabled;
		}

		public async Task Stop()
		{
			Process? waitForProcess = StartProcess(CreateWorkerStartInfo($"stop {Bundle.Id}"));
			if (waitForProcess != null)
				await waitForProcess.WaitForExitAsync();
			State = PackageState.None;
		}

		public void Enable()
		{
			// TODO: Enable via PackageManager
			Start();
			State = PackageState.Activated;
		}

		public void Start()
		{
			StartProcess(CreateWorkerStartInfo($"run {Bundle.Id}"));
			State = PackageState.Activated;
		}

		public void Install()
		{
			StartProcess(CreateWorkerStartInfo($"install \"{Bundle.BundleLocation}\""));
			// TODO: Should install require a restart, so we have a clear end/start of install->run?
			State = PackageState.Loaded;
		}

		public void ShowUI()
		{
			StartProcess(CreateWorkerStartInfo($"activate rtk://show-ui?id={Bundle.Id}"));
		}

		public async Task Uninstall()
		{
			State = PackageState.PendingUninstall;
			Process? waitForProcess = StartProcess(CreateWorkerStartInfo($"uninstall rtk://show-ui?id={Bundle.Id}"));
			if (waitForProcess != null)
				await waitForProcess.WaitForExitAsync();

			Dependencies.GetRequiredService<IPackageManager>().RemovePackage(Bundle.Id);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					Job.Dispose();
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
}
