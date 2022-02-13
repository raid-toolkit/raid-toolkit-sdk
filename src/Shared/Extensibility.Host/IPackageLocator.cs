using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility
{
    public interface IPackageLocator
    {
        public IEnumerable<PackageDescriptor> GetAllPackages();
        public PackageDescriptor GetPackage(string packageId);
    }
}
