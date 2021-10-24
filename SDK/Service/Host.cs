using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Raid.Service.DataModel;
using RaidExtractor.Core;
using SuperSocket.WebSocket.Server;

namespace Raid.Service
{
    public static class RaidHost
    {
        private static IHost Host;

        private static IHostBuilder CreateHostBuilder() => WebSocketHostBuilder.Create()
            .UseWebSocketMessageHandler(ModelService.HandleMessage)
            .ConfigureServices((ctx, services) => services
                .Configure<AppSettings>(opts => ctx.Configuration.GetSection("app").Bind(opts))
                .AddHostedServiceSingleton<MainService>()
                .AddHostedServiceSingleton<ProcessWatcher>()
                .AddHostedServiceSingleton<ChannelService>()
                .AddSingleton<ModelService>()
                .AddSingleton<UserData>()
                .AddSingleton<StaticDataCache>()
                .AddSingleton<RaidInstanceFactory>()
                .AddSingleton<Extractor>()
                .AddScoped<RaidInstance>()
                .AddTypesAssignableTo<IMessageScopeHandler>(collection => collection.AddSingleton)
                .AddTypesAssignableToFactories<IStaticFacet>(collection => collection.AddSingleton)
                .AddTypesAssignableToFactories<IAccountFacet>(collection => collection.AddSingleton)
            )
            .ConfigureLogging(configureLogging => configureLogging.ClearProviders().AddDebug().AddConsole(config => { config.IncludeScopes = true; }));

        public static IHost CreateHost()
        {
            return Host = CreateHostBuilder().Build();
        }

        public static IServiceProvider Services => Host.Services;
    }
}