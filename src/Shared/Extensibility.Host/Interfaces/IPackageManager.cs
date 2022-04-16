using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility.Host
{
    public interface IPackageManager
    {
        public IEnumerable<ExtensionBundle> GetAllPackages();
        public ExtensionBundle GetPackage(string packageId);
        public ExtensionBundle AddPackage(ExtensionBundle package);
    }
}
