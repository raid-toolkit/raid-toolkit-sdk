using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Raid.Toolkit.App;
using Raid.Toolkit.App.Tasks.Base;
using Raid.Toolkit.DependencyInjection;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.Forms;

using SuperSocket.WebSocket;
using SuperSocket.WebSocket.Server;

namespace Raid.Toolkit.Preamble
{
    internal class AppHostBuilder : HostBuilder
    {
        [DllImport("Microsoft.ui.xaml.dll")]
        private static extern void XamlCheckProcessRequirements();

        [Flags]
        private enum Feature
        {
            None = 0,
            WebSocket = (1 << 0),
            UI = (1 << 1),
            Extensibility = (1 << 2),
            Logging = (1 << 3),
            AppServices = (1 << 4),
        }
        private Feature AddedFeatures = Feature.None;

        private bool TryAddFeature(Feature feature)
        {
            if (AddedFeatures.HasFlag(feature))
                return false;
            AddedFeatures |= feature;
            return true;
        }

        public AppHostBuilder()
            : base()
        {
            // default to always include logging, since it is only enabled once configured
            AddLogging();
        }

        public AppHostBuilder AddAppServices()
        {
            if (!TryAddFeature(Feature.AppServices))
                return this;

            return (AppHostBuilder)ConfigureServices((context, services) => services
                .AddSingleton<AppService>()
                .AddFeatures(HostFeatures.ProcessWatcher | HostFeatures.RefreshData)
                );
        }

        public AppHostBuilder AddExtensibility()
        {
            if (!TryAddFeature(Feature.Extensibility))
                return this;

            return (AppHostBuilder)ConfigureServices((context, services) => services
                .AddExtensibilityServices<PackageManager>()
                );
        }

        public AppHostBuilder AddLogging()
        {
            if (!TryAddFeature(Feature.Logging))
                return this;

            return (AppHostBuilder)ConfigureServices((context, services) => services
                .AddLogging(builder => builder.AddFile())
                );
        }

        public AppHostBuilder AddUI()
        {
            if (!TryAddFeature(Feature.UI))
                return this;

            XamlCheckProcessRequirements();

            return (AppHostBuilder)this
                .AddAppServices()
                .ConfigureServices((context, services) => services
                    .AddSingleton<IAppUI, AppWinUI>()
                    .AddHostedServiceSingleton<AppTray>()
                );
        }

        public AppHostBuilder AddWebSockets(Func<WebSocketSession, WebSocketPackage, ValueTask> messageHandler)
        {
            if (!TryAddFeature(Feature.WebSocket))
                return this;

            this.AsWebSocketHostBuilder()
                .UseSessionFactory<SessionFactory>()
                .UseWebSocketMessageHandler(messageHandler)
                .ConfigureAppConfiguration(config => config
                    .AddJsonStream(AppHost.GetEmbeddedSettings())
                    .AddJsonFile(Path.Combine(AppHost.ExecutableDirectory, "appsettings.json"), true)
                );
            return this;
        }
    }
}
