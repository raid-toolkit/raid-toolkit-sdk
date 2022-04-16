using System;

namespace Raid.Toolkit.Extensibility.Host
{
    public interface IPackageInstanceFactory
    {
        public IExtensionPackage CreateInstance(Type type);
    }
}
