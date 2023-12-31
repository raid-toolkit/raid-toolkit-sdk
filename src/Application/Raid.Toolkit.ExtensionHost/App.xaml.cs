using CustomExtensions.WinUI;

using Google.Protobuf.WellKnownTypes;

using Karambolo.Extensions.Logging.File;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;

using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.Extensibility.Host.Utils;
using Raid.Toolkit.Extensibility.Interfaces;
using Raid.Toolkit.ExtensionHost.ViewModel;
using Raid.Toolkit.Loader;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using WinUIEx;

namespace Raid.Toolkit.ExtensionHost;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
	private const string LogDir = "Logs";

	public static new App Current => Application.Current as App ?? throw new NullReferenceException();

	public IAppDispatcher Dispatcher { get; }
	public IServiceProvider ServiceProvider { get; }
	public string PackageId { get; }
	public ExtensionHostModel Model { get; }

	private Window? m_window;
	private readonly IHost Host;
	private readonly BaseOptions InitialOptions;
	private readonly Queue<BaseOptions> ActivationRequests = new();

	public App(BaseOptions initialOptions)
	{
		InitialOptions = initialOptions;

		this.InitializeComponent();
		ApplicationExtensionHost.Initialize(this);

		HostBuilder hostBuilder = new();

		if (!initialOptions.DisableLogging)
		{
			hostBuilder
				.ConfigureLogging((_) => GetLoggerOptions(initialOptions))
				.ConfigureServices((context, services) => services.AddLogging(builder => builder.AddFile()));
		}

		// logging
		hostBuilder
			.ConfigureLogging((_) => GetLoggerOptions(initialOptions))
			.ConfigureServices((context, services) => services
				.AddLogging(builder => builder.AddFile())
				.Configure<ModelLoaderOptions>(config => config.ForceRebuild = initialOptions.ForceRebuild)
				.AddSingleton<IPackageManager, PackageManager>()
				.AddSingleton<IAppDispatcher, AppDispatcher>()
				.AddScoped<IModelLoader, ModelLoader>()
				.AddScoped<IMenuManager, ClientMenuManager>()
				.AddScoped<IWindowManager, Extensibility.Host.WindowManager>()
				.AddScoped<IManagedPackageFactory, ManagedPackageFactory>() // creates a scope for each IExtensionManagement
				.AddScoped<IPackageLoader, SandboxedPackageLoader>()
				.AddScoped(sp => CreateExtensionManagementScope(sp, initialOptions.GetPackageId()))
			);
		Host = hostBuilder.Build();

		// force AppUI to initialize on the app ui thread so it can capture the SynchronizationContext
		Dispatcher = Host.Services.GetRequiredService<IAppDispatcher>();
		ServiceProvider = Host.Services;
		PackageId = initialOptions.GetPackageId();
		Model = new();
	}

	protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
	{
		m_window = ActivatorUtilities.CreateInstance<MainWindow>(Host.Services);
		m_window.Hide();
		Model.MainWindow = m_window;

		while (ActivationRequests.TryDequeue(out BaseOptions? options))
			DoActivation(options);
	}

	internal void OnActivated(BaseOptions options)
	{
		if (options.GetPackageId() != InitialOptions.GetPackageId())
			throw new InvalidOperationException("One worker cannot serve multiple packages");

		// defer activation processing until after the main window has been created
		if (m_window != null)
		{
			DoActivation(options);
		}
		else
		{
			ActivationRequests.Enqueue(options);
		}
	}

	private void DoActivation(BaseOptions options)
	{
		switch (options)
		{
			case RunPackageOptions runOptions:
				Task.Run(async () =>
				{
					IManagedPackage extension = await InitializePackage(runOptions);
					await RunPackage(extension);
				});
				break;
			case InstallPackageOptions installPackageOptions:
				Task.Run(() => InstallPackage(installPackageOptions));
				break;
		}
	}

	private async Task InstallPackage(InstallPackageOptions installPackageOptions)
	{
		ExtensionBundle bundleToInstall = ExtensionBundle.FromFile(installPackageOptions.PackagePath);
		try
		{
			await Model.RequestUserTrust(bundleToInstall);
		}
		catch (OperationCanceledException)
		{
			// terminate
			ServiceProvider.GetRequiredService<IAppDispatcher>().Dispatch(() => m_window?.Close());
			return;
		}
		Model.StartProgress($"Installing {bundleToInstall.Manifest.DisplayName}...");

		Model.UpdateProgress(10, "Copying files...");
		await Task.Delay(3000);
		IPackageManager packageManager = ServiceProvider.GetRequiredService<IPackageManager>();
		ExtensionBundle? installedBundle = packageManager.InstallPackage(bundleToInstall);

		if (installedBundle == null)
		{
			// TODO: If files are in-use, restart this instance
			throw new V3NotImpl();
		}

		RunPackageOptions runPackageOptions = new() { PackageId = installedBundle.Id };
		IModelLoader modelLoader = ServiceProvider.GetRequiredService<IModelLoader>();
		modelLoader.OnStateUpdated += Loader_OnStateUpdated;
		IManagedPackage extension = await InitializePackage(runPackageOptions);
		Model.EndProgress(true);
		await Task.Delay(3000);
		Model.Hide();

		await RunPackage(extension);
	}

	private void Loader_OnStateUpdated(object? sender, IModelLoader.ModelLoaderEventArgs e)
	{
		switch (e.LoadState)
		{
			case IModelLoader.LoadState.Initialize:
				Model.UpdateProgress(20, "Initializing model...");
				break;
			case IModelLoader.LoadState.Rebuild:
				Model.UpdateProgress(20, "Rebuilding model...");
				break;
			case IModelLoader.LoadState.Ready:
				Model.UpdateProgress(95, "Model is ready");
				break;
			case IModelLoader.LoadState.Loaded:
				Model.UpdateProgress(99, "Activating extension");
				break;
			case IModelLoader.LoadState.Error:
				Model.UpdateProgress(null, "An error occurred during installation");
				break;
			default:
				break;
		}
	}


	private async Task RunPackage(IManagedPackage extension)
	{
		if (extension.State == ExtensionState.None)
		{
			await extension.Load();
		}
		if (extension.State == ExtensionState.Loaded)
		{
			extension.Activate();
		}
	}

	private async Task<IManagedPackage> InitializePackage(RunPackageOptions options)
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

		await Host.StartAsync();
		IServiceScope packageScope = Host.Services.CreateScope();
		IManagedPackage extension = packageScope.ServiceProvider.GetRequiredService<IManagedPackage>();

		if (extension.State == ExtensionState.Disabled)
			throw new InvalidOperationException("Extension is disabled");
		if (extension.State == ExtensionState.Error)
			throw new InvalidOperationException("Extension is in an error state");
		if (extension.State == ExtensionState.PendingUninstall)
			throw new InvalidOperationException("Extension is pending uninstallation");
		return extension;
	}

	private static IManagedPackage CreateExtensionManagementScope(IServiceProvider provider, string packageId)
	{
		IPackageManager packageManager = provider.GetRequiredService<IPackageManager>();
		ExtensionBundle package = packageManager.GetPackage(packageId);
		return ActivatorUtilities.CreateInstance<Extensibility.Host.ManagedPackage>(provider, package);
	}

	private static FileLoggerOptions GetLoggerOptions(BaseOptions options)
	{
		if (!Directory.Exists(RegistrySettings.InstallationPath))
		{
			return new();
		}
		string logDir = Path.Combine(RegistrySettings.InstallationPath, LogDir);
		DirectoryInfo dir = Directory.CreateDirectory(logDir);

		IEnumerable<FileInfo> existingFiles = dir.GetFiles().Where(file => file.CreationTimeUtc < DateTime.UtcNow.AddDays(-2));
		foreach (FileInfo file in existingFiles)
			file.Delete();

		string logFileNameFormat = $"Extension.{options.GetPackageId()}.<date:yyyyMMdd>-<counter>.log";

		PhysicalFileProvider fileProvider = new(RegistrySettings.InstallationPath);
		FileLoggerOptions loggerOptions = new()
		{
			FileAppender = new PhysicalFileAppender(fileProvider),
			BasePath = LogDir,
			FileAccessMode = LogFileAccessMode.KeepOpenAndAutoFlush,
			FileEncodingName = "utf-8",
			DateFormat = "yyyyMMdd",
			CounterFormat = "000",
			MaxFileSize = 10485760,
			IncludeScopes = true,

			TextBuilder = new SingleLineLogEntryTextBuilder(),
			Files = new[] { new LogFileOptions { Path = logFileNameFormat } },
		};
		return loggerOptions;
	}
}
