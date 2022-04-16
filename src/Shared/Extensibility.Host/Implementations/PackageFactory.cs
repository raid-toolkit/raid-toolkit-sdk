using System;
using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Extensibility.Host;

namespace Raid.Toolkit.Extensibility
{
    public class PackageFactory : IPackageInstanceFactory
    {
        private readonly IServiceProvider ServiceProvider;

        public PackageFactory(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

        public IExtensionPackage CreateInstance(Type type)
        {
            var scope = ServiceProvider.CreateScope().ServiceProvider;
            return (IExtensionPackage)ActivatorUtilities.CreateInstance(scope, type);
        }
    }

}
