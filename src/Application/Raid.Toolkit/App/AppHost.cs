using System;
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
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.Server;

namespace Raid.Toolkit
{
    class Settings : IDataServiceSettings
    {
        public string InstallationPath => ".";
        public string StoragePath => @".\Data";
    }

    internal class AppHost
    {
        public static readonly string ExecutablePath;
        public static readonly string ExecutableName = "Raid.Toolkit.exe";
        public static readonly string ExecutableDirectory;

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
            throw new NotImplementedException();
            // string? origin = session.HttpHeader.Items.Get("origin");
            // if (!string.IsNullOrEmpty(origin))
            // {
            //     if (!Host!.Services.GetRequiredService<UI.MainWindow>().RequestPermissions(origin))
            //     {
            //         session.CloseAsync(CloseReason.ViolatePolicy, "User denied access");
            //     }
            // }
            // return Host.Services.GetRequiredService<ModelService>()
            //     .ProcessMessage(new SuperSocketAdapter(session), message);
        }

        private static IHostBuilder CreateHostBuilder() =>
            WebSocketHostBuilder.Create()
            .UseSessionFactory<SessionFactory>()
            .UseWebSocketMessageHandler(HandleMessage)
            .ConfigureAppConfiguration(config => config
                .AddJsonStream(GetEmbeddedSettings())
                .AddJsonFile(Path.Join(ExecutableDirectory, "appsettings.json"), true)
            )
            .ConfigureServices((context, services) => services
                // app dependencies
                .Configure<ProcessManagerSettings>(config =>
                {
                    config.PollIntervalMs = 100;
                    config.ProcessName = "Raid";
                })
                .AddLogging(builder =>
                {
                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                    builder.AddFile(o => o.RootPath = RegistrySettings.InstallationPath);
                })
                .AddSingleton<IDataServiceSettings, Settings>()
                .AddTypesAssignableTo<ICommandTask>(services => services.AddScoped)
                .AddSingleton<ApplicationStartupTask>()
                // shared dependencies
                .AddExtensibilityServices<PackageManager>()
                .AddFeatures(HostFeatures.ProcessWatcher)
            );
    }
}