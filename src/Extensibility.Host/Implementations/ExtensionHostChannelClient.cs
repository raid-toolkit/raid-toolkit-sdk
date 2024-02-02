using Raid.Toolkit.Common.API;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host;

public class ExtensionHostChannelClient : ApiCallerBase<IExtensionHostChannel>, IExtensionHostChannel
{
	public ExtensionHostChannelClient(IWorkerApplication workerApplication)
		: base(workerApplication.Client)
	{
	}

	public event EventHandler<ManifestLoadedEventArgs> ManifestLoaded
	{
		add => AddHandler(nameof(ManifestLoaded), value);
		remove => RemoveHandler(nameof(ManifestLoaded), value);
	}

	public Task<bool> ReloadManifest(string packageId)
	{
		return CallMethod<bool>(nameof(ReloadManifest), packageId);
	}
}
