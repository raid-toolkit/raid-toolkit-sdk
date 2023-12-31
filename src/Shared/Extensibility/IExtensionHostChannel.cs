using Raid.Toolkit.Common.API;

using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility
{
	[PublicApi(nameof(IExtensionHostChannel))]
	public interface IExtensionHostChannel
	{
		[PublicApi(nameof(ReloadManifest))]
		public Task<bool> ReloadManifest(string packageId);
	}
}
