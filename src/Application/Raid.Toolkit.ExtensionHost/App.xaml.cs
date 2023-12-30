using CustomExtensions.WinUI;

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
				.AddScoped<IModelLoader, ModelLoader>()
				.AddScoped<IMenuManager, ClientMenuManager>()
				.AddScoped<IWindowManager, Extensibility.Host.WindowManager>()
				.AddScoped<IManagedPackageFactory, ManagedPackageFactory>() // creates a scope for each IExtensionManagement
				.AddScoped<IPackageLoader, SandboxedPackageLoader>()
				.AddScoped(sp => CreateExtensionManagementScope(sp, initialOptions.GetPackageId()))
			);
		Host = hostBuilder.Build();
	}

	protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
	{
		m_window = ActivatorUtilities.CreateInstance<MainWindow>(Host.Services);
		m_window.Hide();

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
				Task.Run(() => RunPackage(runOptions));
				break;
		}
	}

	private async Task RunPackage(RunPackageOptions options)
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

		if (extension.State == ExtensionState.None)
		{
			await extension.Load();
		}
		if (extension.State == ExtensionState.Loaded)
		{
			extension.Activate();
		}
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
