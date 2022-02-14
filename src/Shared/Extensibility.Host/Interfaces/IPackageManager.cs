using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility.Host
{
    public interface IPackageManager
    {
        public IEnumerable<PackageDescriptor> GetAllPackages();
        public PackageDescriptor GetPackage(string packageId);
        public PackageDescriptor AddPackage(PackageDescriptor package);
    }
}
