using System;
using System.IO;
using System.Threading.Tasks;

using ECS.Components;

using Karambolo.Extensions.Logging.File;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Raid.Toolkit.App.Tasks;
using Raid.Toolkit.App.Tasks.Base;
using Raid.Toolkit.Common;
using Raid.Toolkit.DependencyInjection;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.Extensibility.Host.Services;
using Raid.Toolkit.Extensibility.Services;
using Raid.Toolkit.Extensibility.Shared;
using Raid.Toolkit.Forms;
using Raid.Toolkit.Utility;


using SuperSocket.WebSocket;
using SuperSocket.WebSocket.Server;

namespace Raid.Toolkit.App
{
    internal class AppHost
    {
        private const string LogDir = "Logs";
        public static readonly string ExecutablePath;
        public static readonly string ExecutableName = "Raid.Toolkit.exe";
        public static string ExecutableDirectory;
        public static DeferredOptions<FileLoggerOptions> FileLoggerOptions;

        public static void EnsureFileAssociations(string? exePath = null)
        {
            if (string.IsNullOrEmpty(exePath))
                exePath = ExecutablePath;
            FileAssociations.EnsureAssociationsSet(new FileAssociation()
            {
                Extension = ".rtkx",
                FileTypeDescription = "Raid Toolkit Extension",
                ExecutableFilePath = exePath,
                ExecutableArguments = "install",
                ProgId = "RTK.Extension"
            });
        }

        internal static Stream? GetEmbeddedSettings()
        {
            var assembly = typeof(AppHost).Assembly;
            return assembly.GetManifestResourceStream($"Raid.Toolkit.appsettings.json");
        }

        private static bool _EnableLogging = false;
        public static bool EnableLogging
        {
            get => _EnableLogging;
            set
            {
                if (value == _EnableLogging)
                    return;

                _EnableLogging = value;
                if (!_EnableLogging)
                {
                    FileLoggerOptions.Set(new());
                    return;
                }
                if (Directory.Exists(RegistrySettings.InstallationPath))
                {
                    Directory.CreateDirectory(Path.Combine(RegistrySettings.InstallationPath, LogDir));
                    PhysicalFileProvider fileProvider = new(RegistrySettings.InstallationPath);
                    FileLoggerOptions options = new()
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
                        Files = new[]
                        {
                        new LogFileOptions
                        {
                            Path = "<date:yyyyMMdd>-<counter>.log",
                        },
                    },
                    };
                    FileLoggerOptions.Set(options);
                }
            }
        }

        static AppHost()
        {
            ExecutablePath = System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName!;
            ExecutableDirectory = Path.GetDirectoryName(ExecutablePath)!;
            FileLoggerOptions = new(new());
        }

        private static IHost? Host;

        public static IHost CreateHost()
        {
            return Host = CreateHostBuilder().Build();
        }

        public static void Start(IHost host)
        {
            Host = host;
        }

        /**
         * Handles messages from the public socket API
        **/
        public static ValueTask HandleMessage(WebSocketSession session, WebSocketPackage message)
        {
            return Host == null
                ? ValueTask.CompletedTask
                : Host.Services.GetRequiredService<IScopedServiceManager>()
                .ProcessMessage(new SuperSocketAdapter(session), message.Message);
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return new HostBuilder()
                //WebSocketHostBuilder.Create()
                //.UseSessionFactory<SessionFactory>()
                //.UseWebSocketMessageHandler(HandleMessage)
                //.ConfigureAppConfiguration(config => config
                //    .AddJsonStream(GetEmbeddedSettings())
                //    .AddJsonFile(Path.Combine(ExecutableDirectory, "appsettings.json"), true)
                //)
                .ConfigureServices((context, services) => services
                    // app dependencies
                    .Configure<AppSettings>(opts => context.Configuration.GetSection("app").Bind(opts))
                    .Configure<ProcessManagerSettings>(opts => context.Configuration.GetSection("app:ProcessManager").Bind(opts))
                    .Configure<DataUpdateSettings>(opts => context.Configuration.GetSection("app:DataSettings").Bind(opts))
                    .Configure<StorageSettings>(opts => context.Configuration.GetSection("app:StorageSettings").Bind(opts))
                    .AddLogging(builder => builder.AddFile())
                    .AddSingleton<IAppUI, AppWinUI>()
                    .AddTypesAssignableTo<ICommandTask>(services => services.AddScoped)
                    .AddSingleton<CommandTaskManager>()
                    .AddSingleton<AppService>()
                    .AddSingleton<IOptionsMonitor<FileLoggerOptions>>(FileLoggerOptions)
                    .AddHostedServiceSingleton<AppTray>()
                    // shared dependencies
                    .AddExtensibilityServices<PackageManager>()
                    .AddFeatures(HostFeatures.ProcessWatcher | HostFeatures.RefreshData)
                );
        }
    }
}
