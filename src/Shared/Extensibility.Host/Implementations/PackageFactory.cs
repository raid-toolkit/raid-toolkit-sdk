using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Extensibility.Host;
using System;

namespace Raid.Toolkit.Extensibility
{
    public class PackageFactory : IPackageInstanceFactory
    {
        private readonly IServiceProvider ServiceProvider;

        public PackageFactory(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

        public IExtensionPackage CreateInstance(Type type, PackageDescriptor descriptor)
        {
            var scope = ServiceProvider.CreateScope().ServiceProvider;
            ActivatorUtilities.CreateInstance<PackageContext>(scope, descriptor);
            return (IExtensionPackage)ActivatorUtilities.CreateInstance(scope, type);
        }
    }

}
