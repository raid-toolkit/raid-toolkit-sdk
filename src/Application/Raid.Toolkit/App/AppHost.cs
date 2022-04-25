using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.Extensibility.Host.Services;
using Raid.Toolkit.Extensibility.Services;
using Raid.Toolkit.Extensibility.Shared;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.Server;

namespace Raid.Toolkit
{
    internal class AppHost
    {
        public static readonly string ExecutablePath;
        public static readonly string ExecutableName = "Raid.Toolkit.exe";
        public static string ExecutableDirectory;

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
            return assembly.GetManifestResourceStream($"{assembly.GetName().Name}.appsettings.json");
        }

        static AppHost()
        {
            ExecutablePath = System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName!;
            ExecutableDirectory = Path.GetDirectoryName(ExecutablePath)!;
        }

        private static IHost? Host;

        public static IHost CreateHost()
        {
            return Host = CreateHostBuilder().Build();
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
            return WebSocketHostBuilder.Create()
            .UseSessionFactory<SessionFactory>()
            .UseWebSocketMessageHandler(HandleMessage)
            .ConfigureAppConfiguration(config => config
                .AddJsonStream(GetEmbeddedSettings())
                .AddJsonFile(Path.Combine(ExecutableDirectory, "appsettings.json"), true)
            )
            .ConfigureServices((context, services) => services
                // app dependencies
                .Configure<AppSettings>(opts => context.Configuration.GetSection("app").Bind(opts))
                .Configure<ProcessManagerSettings>(opts => context.Configuration.GetSection("app:ProcessManager").Bind(opts))
                .Configure<DataUpdateSettings>(opts => context.Configuration.GetSection("app:DataSettings").Bind(opts))
                .Configure<StorageSettings>(opts => context.Configuration.GetSection("app:StorageSettings").Bind(opts))
                .AddLogging(builder =>
                {
                    if (Directory.Exists(RegistrySettings.InstallationPath))
                    {
                        _ = builder.AddConfiguration(context.Configuration.GetSection("Logging"));

                        Directory.CreateDirectory(Path.Combine(RegistrySettings.InstallationPath, "Logs"));
                        _ = builder.AddFile(o => o.RootPath = RegistrySettings.InstallationPath);
                    }
                })
                .AddTypesAssignableTo<ICommandTask>(services => services.AddScoped)
                .AddSingleton<ApplicationStartupTask>()
                .AddSingleton<AppService>()
                // shared dependencies
                .AddExtensibilityServices<PackageManager>()
                .AddFeatures(HostFeatures.ProcessWatcher | HostFeatures.RefreshData)
            );
        }
    }
}
