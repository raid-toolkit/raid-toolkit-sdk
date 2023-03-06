using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host
{
    public interface IPackageLoader
    {
        public Task<IExtensionPackage> LoadPackage(ExtensionBundle bundle);
    }
}
