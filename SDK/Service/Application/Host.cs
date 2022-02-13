using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Raid.Service.Services;
using RaidExtractor.Core;
using SuperSocket.WebSocket.Server;

namespace Raid.Service
{
    public static class RaidHost
    {
        private static IHost Host;

        internal static Stream GetEmbeddedSettings()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream($"{assembly.GetName().Name}.appsettings.json");
        }

        private static IHostBuilder CreateHostBuilder(RunOptions options) => WebSocketHostBuilder.Create()
            .UseSessionFactory<SessionFactory>()
            .UseWebSocketMessageHandler(ModelService.HandleMessage)
            .ConfigureAppConfiguration(config => config
                .AddJsonStream(GetEmbeddedSettings())
                .AddJsonFile(Path.Join(AppConfiguration.ExecutableDirectory, "appsettings.json"), true)
            )
            .ConfigureServices((ctx, services) => services
                .Configure<AppSettings>(opts => ctx.Configuration.GetSection("app").Bind(opts))
                .AddLogging(builder =>
                {
                    builder.AddConfiguration(ctx.Configuration.GetSection("Logging"));
                    builder.AddFile(o => o.RootPath = AppConfiguration.InstallationPath);
                })
                .AddHttpClient()
                .AddHostedServiceSingleton<MainService>()
                .AddHostedServiceSingleton<ProcessWatcherService>()
                .AddHostedServiceSingleton<ChannelService>()
                .AddHostedServiceSingleton<UpdateService>()
                .AddHostedServiceSingleton<ErrorService>()
                .AddHostedServiceSingleton<EventService>()
                .AddHostedServiceSingleton<FrameRateService>()
                .AddSingleton<ModelService>()
                .AddSingleton<AppData>()
                .AddSingleton<RaidInstanceFactory>()
                .AddSingleton<Extractor>()
                .AddSingleton<MemoryLogger>()
                .AddSingleton<GitHub.Updater>()
                .AddSingleton<RunOptions>(options)
                .AddScoped<UI.MainWindow>()
                .AddScoped<UI.ErrorsWindow>()
                .AddScoped<RaidInstance>()
                .AddAppModel()
                .AddTypesAssignableTo<IMessageScopeHandler>(collection => collection.AddSingleton)
            );

        public static IHost CreateHost(RunOptions options)
        {
            return Host = CreateHostBuilder(options).Build();
        }

        public static IServiceProvider Services => Host.Services;
    }
}
