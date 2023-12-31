using Raid.Toolkit.Common.API;
using Raid.Toolkit.Extensibility.Interfaces;

using System.Reflection;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host
{
	public class ExtensionHostChannelClient : ApiCallerBase<IExtensionHostChannel>, IExtensionHostChannel
	{
		public ExtensionHostChannelClient(IWorkerApplication workerApplication)
			: base(workerApplication.Client)
		{ }

		public Task<bool> ReloadManifest(string packageId)
		{
			return CallMethod<bool>(MethodBase.GetCurrentMethod()!, packageId);
		}
	}
}
