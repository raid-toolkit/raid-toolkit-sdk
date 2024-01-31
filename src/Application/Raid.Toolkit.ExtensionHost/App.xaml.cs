using CustomExtensions.WinUI;

using Karambolo.Extensions.Logging.File;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Host.Server;
using Raid.Toolkit.Extensibility.Host.Services;
using Raid.Toolkit.Extensibility.Host.Utils;
using Raid.Toolkit.ExtensionHost.ViewModel;
using Raid.Toolkit.Loader;

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
		Host = BuildHost(initialOptions);
		Model = new(initialOptions, Host.Services);
	}

	private static IHost BuildHost(BaseOptions initialOptions)
	{
		HostBuilder hostBuilder = new();

		if (!initialOptions.DisableLogging)
		{
			hostBuilder
				.ConfigureLogging((_) => GetLoggerOptions(initialOptions))
				.ConfigureServices((context, services) => services.AddLogging(builder => builder.AddFile()));
		}

		if (initialOptions.Standalone)
			hostBuilder.ConfigureServices((context, services) => services
				.AddHostedServiceSingleton<IServerApplication, ServerApplication>()
				.AddHostedServiceSingleton<IExtensionHostChannel, ExtensionHostChannelServer>()
				.AddHostedServiceSingleton<IRuntimeManager, RuntimeManagerServer>()
				.AddHostedServiceSingleton<IMenuManager, MenuManagerServer>());
		else
			hostBuilder.ConfigureServices((context, services) => services
				.AddHostedServiceSingleton<IWorkerApplication, WorkerApplication>()
				.AddSingleton<IExtensionHostChannel, ExtensionHostChannelClient>()
				.AddSingleton<IRuntimeManager, RuntimeManagerClient>()
				.AddSingleton<IMenuManager, MenuManagerClient>());

		// logging
		hostBuilder
			.ConfigureLogging((_) => GetLoggerOptions(initialOptions))
			.ConfigureServices((context, services) => services
				.AddLogging(builder => builder.AddFile())
				.Configure<ModelLoaderOptions>(config => config.ForceRebuild = initialOptions.ForceRebuild)
				.AddHostedServiceSingleton<IGameInstanceManager, GameInstanceManager>()
				.AddHostedServiceSingleton<IAccountManager, AccountManager>()
				.AddHostedServiceSingleton<IDataStorageReaderWriter, FileStorageService>()
				.AddSingleton(typeof(CachedDataStorage))
				.AddSingleton(typeof(CachedDataStorage<>))
				.AddSingleton<PersistedDataStorage>()
				.AddSingleton<IPackageManager, PackageManager>()
				.AddSingleton<IAppDispatcher, AppDispatcher>()
				.AddSingleton<IModelLoader, ModelLoader>()
				.AddSingleton<IWindowManager, WindowManager>()
				.AddSingleton<IManagedPackageFactory, ManagedPackageFactory>() // creates a scope for each IExtensionManagement
				.AddSingleton<IPackageLoader, SandboxedPackageLoader>()
				.AddSingleton<IManagedPackage>(sp => CreateExtensionManagementScope(sp, initialOptions.GetPackageId()))
			);
		return hostBuilder.Build();
	}

	protected override void OnLaunched(LaunchActivatedEventArgs args)
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
		return ActivatorUtilities.CreateInstance<ManagedPackage>(provider, package);
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
