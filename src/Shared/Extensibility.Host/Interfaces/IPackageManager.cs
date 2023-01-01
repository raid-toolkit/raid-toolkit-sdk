using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host
{
    public interface IPackageManager
    {
        public IEnumerable<ExtensionBundle> GetAllPackages();
        public ExtensionBundle GetPackage(string packageId);
        public ExtensionBundle? AddPackage(ExtensionBundle package);
        public Task<ExtensionBundle?> RequestPackageInstall(ExtensionBundle package);
        public void RemovePackage(string packageId);
    }
}
