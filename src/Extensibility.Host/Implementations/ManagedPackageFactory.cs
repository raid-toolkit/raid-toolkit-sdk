using System;
using Microsoft.Extensions.DependencyInjection;

namespace Raid.Toolkit.Extensibility.Host;
public class ManagedPackageFactory : IManagedPackageFactory
{
	private readonly IServiceProvider ServiceProvider;

	public ManagedPackageFactory(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

	public IExtensionPackage CreateInstance(Type type)
	{
		var scope = ServiceProvider.CreateScope().ServiceProvider;
		return (IExtensionPackage)ActivatorUtilities.CreateInstance(scope, type);
	}
}
