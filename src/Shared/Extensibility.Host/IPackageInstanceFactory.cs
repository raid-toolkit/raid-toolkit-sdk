using System;

namespace Raid.Toolkit.Extensibility
{
    public interface IPackageInstanceFactory
    {
        public IExtensionPackage CreateInstance(Type type);
    }
}
