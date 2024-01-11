using Karambolo.Extensions.Logging.File;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Raid.Toolkit.Extensibility.Host.Server;
using Raid.Toolkit.Extensibility.Host.Utils;
using Raid.Toolkit.Extensibility.Notifications;
using Raid.Toolkit.Loader;
using Raid.Toolkit.Model;
using Raid.Toolkit.UI.WinUI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Raid.Toolkit;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
partial class RTKApplication : Application
{
	private const string LogDir = "Logs";
	public static readonly string ExecutablePath;
	public static readonly string ExecutableName = "Raid.Toolkit.exe";
	public static readonly string WorkerExecutableName = "Raid.Toolkit.ExtensionHost.exe";
	public static readonly string ExecutableDirectory;
	public static string LogsDirectory => Path.Combine(RegistrySettings.InstallationPath, LogDir);

	static RTKApplication()
	{
		ExecutablePath = Environment.ProcessPath!;
		ExecutableDirectory = Path.GetDirectoryName(ExecutablePath)!;
	}

	public IHost Host { get; }
	public IApplicationModel Model { get; }
	public StartupOptions Options { get; }

	public static new RTKApplication Current => Application.Current as RTKApplication ?? throw new NullReferenceException();

	/// <summary>
	/// Initializes the singleton application object.  This is the first line of authored code
	/// executed, and as such is the logical equivalent of main() or WinMain().
	/// </summary>
	public RTKApplication(StartupOptions options)
	{
		this.InitializeComponent();
		Options = options;
		Host = BuildHost(options);
		Model = Host.Services.GetRequiredService<IApplicationModel>();
	}

	private IHost BuildHost(StartupOptions initialOptions)
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
				.AddHostedServiceSingleton<IServerApplication, ServerApplication>());

		// logging
		hostBuilder
			.ConfigureLogging((_) => GetLoggerOptions(initialOptions))
			.ConfigureServices((context, services) => services
				.AddLogging(builder => builder.AddFile())
				.Configure<ModelLoaderOptions>(config => config.ForceRebuild = initialOptions.ForceRebuild)
				.AddHostedServiceSingleton<IServerApplication, ServerApplication>()
				.AddHostedServiceSingleton<IExtensionHostChannel, ExtensionHostChannelServer>()
				.AddHostedServiceSingleton<IRuntimeManager, RuntimeManagerServer>()
				.AddHostedServiceSingleton<IMenuManager, MenuManagerServer>()
				.AddHostedServiceSingleton<IAppUI, AppWinUI>()
				.AddHostedServiceSingleton<IUpdateService, UpdateService>()
				.AddHostedServiceSingleton<INotificationManager, NotificationManager>()
				.AddSingleton<GitHub.Updater>()
				.AddSingleton<IPackageManager, PackageManager>()
				.AddSingleton<IAppDispatcher, AppDispatcher>()
				.AddSingleton<IModelLoader, ModelLoader>()
				.AddSingleton<IPackageWorkerManager, PackageWorkerManager>()
				.AddSingleton<IApplicationModel, ApplicationModel>()
			);
		return hostBuilder.Build();
	}


	protected override async void OnLaunched(LaunchActivatedEventArgs args)
	{
		await Host.StartAsync();
		Model.OnLaunched();
	}

	internal void OnActivated(StartupOptions options)
	{
		Model.OnActivated(options);
	}

	private static FileLoggerOptions GetLoggerOptions(StartupOptions options)
	{
		if (!Directory.Exists(RegistrySettings.InstallationPath))
		{
			return new();
		}
		DirectoryInfo dir = Directory.CreateDirectory(LogsDirectory);

		IEnumerable<FileInfo> existingFiles = dir.GetFiles().Where(file => file.CreationTimeUtc < DateTime.UtcNow.AddDays(-2));
		foreach (FileInfo file in existingFiles)
			file.Delete();

		string logFileNameFormat = $"Raid.Toolkit.<date:yyyyMMdd>-<counter>.log";

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
