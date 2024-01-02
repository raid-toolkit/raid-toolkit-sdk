using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Diagnostics.CodeAnalysis;

using Windows.Security.Cryptography.Certificates;

namespace Raid.Toolkit.ExtensionHost.ViewModel;

public partial class ExtensionHostModel : AsyncObservable
{
	private readonly DependencySynthesizer Dependencies;
	private readonly IServiceProvider ServiceProvider;
	private readonly BaseOptions InitialOptions;
	private readonly Queue<BaseOptions> ActivationRequests = new();
	private IManagedPackage? ExtensionInstance;

	public IPackageManager PackageManager => Dependencies.GetRequiredService<IPackageManager>();
	public string PackageId { get; }
	public ExtensionBundle? BundleToInstall { get; private set; }
	public ExtensionBundle? InstalledBundle { get; private set; }
	public IAppDispatcher Dispatcher => Dependencies.GetRequiredService<IAppDispatcher>();


	// must be called on the app ui thread so it can capture the SynchronizationContext
	public ExtensionHostModel(BaseOptions initialOptions, IServiceProvider serviceProvider)
		: base(serviceProvider.GetRequiredService<IAppDispatcher>())
	{
		ServiceProvider = serviceProvider;
		InitialOptions = initialOptions;
		PackageId = initialOptions.GetPackageId();

		Dependencies = new(serviceProvider);
	}

	public void OnLaunched()
	{
		try
		{
			MainWindow = ActivatorUtilities.CreateInstance<MainWindow>(ServiceProvider);

			while (ActivationRequests.TryDequeue(out BaseOptions? options))
				DoActivation(options);
		}
		catch
		{
			MainWindow?.Close();
		}
	}

	internal void OnActivated(BaseOptions options)
	{
		if (options.GetPackageId() != InitialOptions.GetPackageId())
			throw new InvalidOperationException("One worker cannot serve multiple packages");

		// defer activation processing until after the main window has been created
		if (MainWindow != null)
		{
			DoActivation(options);
		}
		else
		{
			ActivationRequests.Enqueue(options);
		}
	}

	private Task DoActivation(BaseOptions options)
	{
		switch (options)
		{
			case RunPackageOptions runOptions:
				return Task.Run(async () =>
				{
					ExtensionInstance = await InitializePackage(runOptions);
					await RunPackage(ExtensionInstance);
				});
			case InstallPackageOptions installOptions:
				return Task.Run(() => InstallPackage(installOptions));
			case ActivateExtensionOptions activateOptions:
				return Task.Run(() => ActivatePackage(activateOptions));
		}
		return Task.CompletedTask;
	}

	[MemberNotNull(nameof(ExtensionInstance))]
	private async Task StartExtensionIfNecessary(BaseOptions options)
	{
		if (ExtensionInstance == null)
		{
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
			ExtensionInstance = await InitializePackage(options);
#pragma warning restore CS8774 // Member must have a non-null value when exiting.
			await RunPackage(ExtensionInstance);
		}
	}

	private async void ActivatePackage(ActivateExtensionOptions activateOptions)
	{
		await StartExtensionIfNecessary(activateOptions);
		ExtensionInstance.HandleRequest(new(activateOptions.Uri));
	}

	private async Task InstallPackage(InstallPackageOptions installPackageOptions)
	{
		ExtensionBundle bundleToInstall = ExtensionBundle.FromFile(installPackageOptions.PackagePath);
		try
		{
			await RequestUserTrust(bundleToInstall);
		}
		catch (OperationCanceledException)
		{
			// terminate
			Close();
			return;
		}
		StartProgress($"Installing {bundleToInstall.Manifest.DisplayName}...");

		// use minimum duration timers to ensure the UI displays for a meaningful amount of time
		Task copyDelay = Task.Delay(1000);
		UpdateProgress(10, "Copying files...");
		IPackageManager packageManager = ServiceProvider.GetRequiredService<IPackageManager>();
		ExtensionBundle? installedBundle = packageManager.InstallPackage(bundleToInstall);
		await copyDelay;

		if (installedBundle == null)
		{
			// TODO: If files are in-use, restart this instance
			throw new V3NotImpl();
		}

		Task modelDelay = Task.Delay(1000);
		RunPackageOptions runPackageOptions = new() { PackageId = installedBundle.Id };
		IModelLoader modelLoader = ServiceProvider.GetRequiredService<IModelLoader>();
		modelLoader.OnStateUpdated += Loader_OnStateUpdated;
		IManagedPackage extension = await InitializePackage(runPackageOptions);
		UpdateProgress(100, "Starting extension...");
		await modelDelay;

		try
		{
			await RunPackage(extension);
			EndProgress(false, $"Installed {bundleToInstall.Manifest.DisplayName}!", "Extension has been activated successfully");
		}
		catch (Exception)
		{
			EndProgress(false, $"Failed to install {bundleToInstall.Manifest.DisplayName}", "An error occurred during extension installation. Please check logs for more information.");
			Close();
		}
	}

	private async Task RunPackage(IManagedPackage extension)
	{
		if (extension.State == PackageState.None)
		{
			await extension.Load();
		}
		if (extension.State == PackageState.Loaded)
		{
			extension.Activate();
		}
	}

	private async Task<IManagedPackage> InitializePackage(BaseOptions options)
	{
		if (options.DebugPackage == ".")
		{
			options.DebugPackage = Environment.GetEnvironmentVariable("DEBUG_PACKAGE_DIR") ?? ".";
		}
		PackageManager.DebugPackage = options.DebugPackage;
		if (!string.IsNullOrEmpty(PackageManager.DebugPackage))
		{
			options.Debug = true;
		}

		await App.Current.Host.StartAsync();

		IServiceScope packageScope = ServiceProvider.CreateScope();
		IManagedPackage extension = packageScope.ServiceProvider.GetRequiredService<IManagedPackage>();

		if (extension.State == PackageState.Disabled)
			throw new InvalidOperationException("Extension is disabled");
		if (extension.State == PackageState.Error)
			throw new InvalidOperationException("Extension is in an error state");
		if (extension.State == PackageState.PendingUninstall)
			throw new InvalidOperationException("Extension is pending uninstallation");
		return extension;
	}
}
