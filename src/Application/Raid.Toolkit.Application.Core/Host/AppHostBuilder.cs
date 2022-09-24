using System.Runtime.InteropServices;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.Application.Core.DependencyInjection;
using Raid.Toolkit.Application.Core.Tasks.Base;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.Extensibility.Host.Services;

using SuperSocket.WebSocket;
using SuperSocket.WebSocket.Server;

namespace Raid.Toolkit.Application.Core.Host
{
    internal interface IAppHostBuilder : IHostBuilder
    {
        IAppHostBuilder AddAppServices();
        IAppHostBuilder AddExtensibility();
        IAppHostBuilder AddLogging();
        IAppHostBuilder AddUI();
        IAppHostBuilder AddWebSockets(Func<WebSocketSession, WebSocketPackage, ValueTask> messageHandler);
    }
    internal class AppHostBuilder<TAppUI> : HostBuilder, IAppHostBuilder
        where TAppUI : class, IAppUI, IHostedService, IDisposable
    {
        [Flags]
        private enum Feature
        {
            None = 0,
            WebSocket = 1 << 0,
            UI = 1 << 1,
            Extensibility = 1 << 2,
            Logging = 1 << 3,
            AppServices = 1 << 4,
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

        public IAppHostBuilder AddAppServices()
        {
            if (!TryAddFeature(Feature.AppServices))
                return this;

            return (IAppHostBuilder)ConfigureServices((context, services) => services
                .AddSingleton<AppService>()
                .AddFeatures(HostFeatures.ProcessWatcher | HostFeatures.RefreshData)
                .Configure<AppSettings>(opts => context.Configuration.GetSection("app").Bind(opts))
                .Configure<ProcessManagerSettings>(opts => context.Configuration.GetSection("app:ProcessManager").Bind(opts))
                .Configure<DataUpdateSettings>(opts => context.Configuration.GetSection("app:DataSettings").Bind(opts))
                .Configure<StorageSettings>(opts => context.Configuration.GetSection("app:StorageSettings").Bind(opts))
                );
        }

        public IAppHostBuilder AddExtensibility()
        {
            if (!TryAddFeature(Feature.Extensibility))
                return this;

            return (IAppHostBuilder)ConfigureServices((context, services) => services
                .AddExtensibilityServices<PackageManager>()
                );
        }

        public IAppHostBuilder AddLogging()
        {
            if (!TryAddFeature(Feature.Logging))
                return this;

            return (IAppHostBuilder)ConfigureServices((context, services) => services
                .AddLogging(builder => builder.AddFile())
                );
        }

        public IAppHostBuilder AddUI()
        {
            if (!TryAddFeature(Feature.UI))
                return this;

            return (IAppHostBuilder)
                AddAppServices()
                .ConfigureServices((context, services) => services
                    .AddHostedServiceSingleton<IAppUI, TAppUI>()
                );
        }

        public IAppHostBuilder AddWebSockets(Func<WebSocketSession, WebSocketPackage, ValueTask> messageHandler)
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
