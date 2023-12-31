using CustomExtensions.WinUI;

using Karambolo.Extensions.Logging.File;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;

using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility.Host.Utils;
using Raid.Toolkit.Extensibility.Interfaces;
using Raid.Toolkit.ExtensionHost.ViewModel;
using Raid.Toolkit.Loader;

using WinUIEx;

namespace Raid.Toolkit.ExtensionHost;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
	private const string LogDir = "Logs";

	public static new App Current => Application.Current as App ?? throw new NullReferenceException();

	public ExtensionHostModel Model { get; }
	public IHost Host { get; }

	public App(BaseOptions initialOptions)
	{
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
		Model = new(initialOptions, Host.Services);
	}

	protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
	{
		Model.OnLaunched();
	}

	internal void OnActivated(BaseOptions options)
	{
		Model.OnActivated(options);
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
