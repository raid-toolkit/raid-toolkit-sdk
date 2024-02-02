using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host;

public interface IPackageLoader
{
	Task<IExtensionPackage> LoadPackage(ExtensionBundle bundle);
}
