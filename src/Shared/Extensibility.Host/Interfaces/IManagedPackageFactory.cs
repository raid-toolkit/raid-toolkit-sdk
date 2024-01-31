using System;

namespace Raid.Toolkit.Extensibility.Host;

public interface IManagedPackageFactory
{
	IExtensionPackage CreateInstance(Type type);
}
