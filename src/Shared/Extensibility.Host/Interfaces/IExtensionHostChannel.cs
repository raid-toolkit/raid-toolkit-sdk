using Raid.Toolkit.Common.API;

using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host;

[PublicApi(nameof(IExtensionHostChannel))]
public interface IExtensionHostChannel
{
	[PublicApi(nameof(ReloadManifest))]
	Task<bool> ReloadManifest(string packageId);
}
