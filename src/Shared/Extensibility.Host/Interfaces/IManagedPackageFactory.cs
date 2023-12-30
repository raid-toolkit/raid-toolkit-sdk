using System;

namespace Raid.Toolkit.Extensibility.Host
{
    public interface IManagedPackageFactory
    {
        public IExtensionPackage CreateInstance(Type type);
    }
}
