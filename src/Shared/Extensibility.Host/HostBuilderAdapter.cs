using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility.Host
{
    public class HostBuilderAdapter : IHostBuilder
    {
        protected IHostBuilder HostBuilder;
        public HostBuilderAdapter(IHostBuilder hostBuilder)
        {
            HostBuilder = hostBuilder;
        }

        public IDictionary<object, object> Properties => HostBuilder.Properties;

        public IHost Build()
        {
            return HostBuilder.Build();
        }

        protected IHostBuilder Wrap(IHostBuilder builder)
        {
            HostBuilder = builder;
            return this;
        }

        public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
            => Wrap(HostBuilder.ConfigureAppConfiguration(configureDelegate));

        public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
            => Wrap(HostBuilder.ConfigureContainer(configureDelegate));

        public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
            => Wrap(HostBuilder.ConfigureHostConfiguration(configureDelegate));

        public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
            => Wrap(HostBuilder.ConfigureServices(configureDelegate));

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
            where TContainerBuilder : notnull
            => Wrap(HostBuilder.UseServiceProviderFactory(factory));

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
            where TContainerBuilder : notnull
            => Wrap(HostBuilder.UseServiceProviderFactory(factory));
    }
}
