using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RaidExtractor.Core;
using SuperSocket.WebSocket.Server;

namespace Raid.Service
{
    public static class RaidHost
    {
        private static IHost Host;

        private static IHostBuilder CreateHostBuilder() => WebSocketHostBuilder.Create()
            .UseWebSocketMessageHandler(ModelService.HandleMessage)
            .ConfigureAppConfiguration(config => config.AddJsonFile(Path.Join(AppConfiguration.ExecutableDirectory, "appsettings.json"), false))
            .ConfigureServices((ctx, services) => services
                .Configure<AppSettings>(opts => ctx.Configuration.GetSection("app").Bind(opts))
                .AddLogging(builder =>
                {
                    builder.AddConfiguration(ctx.Configuration.GetSection("Logging"));
                    builder.AddFile(o => o.RootPath = AppConfiguration.ExecutableDirectory);
                })
                .AddHttpClient()
                .AddHostedServiceSingleton<MainService>()
                .AddHostedServiceSingleton<ProcessWatcherService>()
                .AddHostedServiceSingleton<ChannelService>()
                .AddHostedServiceSingleton<UpdateService>()
                .AddSingleton<ModelService>()
                .AddSingleton<UserData>()
                .AddSingleton<StaticDataCache>()
                .AddSingleton<RaidInstanceFactory>()
                .AddSingleton<Extractor>()
                .AddSingleton<MemoryLogger>()
                .AddSingleton<GitHub.Updater>()
                .AddScoped<RaidInstance>()
                .AddTypesAssignableTo<IMessageScopeHandler>(collection => collection.AddSingleton)
                .AddTypesAssignableToFactories<IStaticFacet>(collection => collection.AddSingleton)
                .AddTypesAssignableToFactories<IAccountFacet>(collection => collection.AddScoped)
            );

        public static IHost CreateHost()
        {
            return Host = CreateHostBuilder().Build();
        }

        public static IServiceProvider Services => Host.Services;
    }
}