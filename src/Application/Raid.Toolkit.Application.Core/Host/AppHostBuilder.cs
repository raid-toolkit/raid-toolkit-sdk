using Karambolo.Extensions.Logging.File;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Raid.Toolkit.Application.Core.Commands.Matchers;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.Extensibility.Interfaces;
using Raid.Toolkit.Loader;

namespace Raid.Toolkit.Application.Core.Host
{
    internal interface IAppHostBuilder : IHostBuilder
    {
        IAppHostBuilder AddAppServices(HostFeatures hostFeatures = HostFeatures.ProcessWatcher | HostFeatures.AutoUpdate);
        IAppHostBuilder AddExtensibility();
        IAppHostBuilder AddLogging();
        IAppHostBuilder AddUI();
    }
    internal class UIOptions
    {
    }

    public class AppHostBuilderSettings
    {
        public static DeferredOptions<RunOptions> RunOptions = new(new());
        public static DeferredOptions<FileLoggerOptions> FileLoggerOptions = new(new());
    }

    internal class AppHostBuilder<TAppUI> : HostBuilderAdapter, IAppHostBuilder
        where TAppUI : class, IAppUI, IHostedService, IDisposable
    {
        [Flags]
        private enum Feature
        {
            None = 0,
            // WebSocket = 1 << 0,
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
            : base(new HostBuilder())
        {
            InitializeComponent();
        }

        public AppHostBuilder(IHostBuilder hostBuilder)
            : base(hostBuilder)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // default to always include logging, since it is only enabled once configured
            _ = AddLogging()
                   .ConfigureAppConfiguration(config => config
                       .AddJsonStream(AppHost.GetEmbeddedSettings())
                       .AddJsonFile(Path.Combine(AppHost.ExecutableDirectory, "appsettings.json"), true)
                   );
        }

        public IAppHostBuilder AddAppServices(HostFeatures hostFeatures = HostFeatures.ProcessWatcher | HostFeatures.AutoUpdate)
        {
            if (TryAddFeature(Feature.AppServices))
            {
                ConfigureServices((context, services) => services
                    .AddSingleton<IAppService, AppService>()
                    .AddFeatures(hostFeatures)
                    .Configure<AppSettings>(opts => context.Configuration.GetSection("app").Bind(opts))
                    .Configure<ProcessManagerSettings>(opts => context.Configuration.GetSection("app:ProcessManager").Bind(opts))
                    .Configure<StorageSettings>(opts => context.Configuration.GetSection("app:StorageSettings").Bind(opts))
                    );
            }
            return this;
        }

        public IAppHostBuilder AddExtensibility()
        {
            if (TryAddFeature(Feature.Extensibility))
            {
                ConfigureServices((context, services) => services
                    .Configure<ModelLoaderOptions>(config => config.ForceRebuild = AppHost.ForceRebuild)
                    .AddExtensibilityServices<PackageManager>(ServiceMode.Server));
            }
            return this;
        }

        public IAppHostBuilder AddLogging()
        {
            if (TryAddFeature(Feature.Logging))
            {
                ConfigureServices((context, services) => services
                   .AddSingleton<IOptionsMonitor<FileLoggerOptions>>(AppHostBuilderSettings.FileLoggerOptions)
                   .AddLogging(builder => builder.AddFile())
                   );
            }
            return this;
        }

        public IAppHostBuilder AddUI()
        {
            if (TryAddFeature(Feature.UI))
            {
                ConfigureServices((context, services) => services
                    .AddHostedServiceSingleton<IAppUI, TAppUI>()
                    );
            }
            return this;
        }
    }
}
