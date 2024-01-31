using Newtonsoft.Json;
using Raid.Toolkit.Common.API;
using Raid.Toolkit.Common.API.Messages;
using System;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host;

public class ManifestLoadedEventArgs : SerializableEventArgs
{
	public string PackageId => (string)EventArguments[0];

	public ManifestLoadedEventArgs(string packageId)
		: base(packageId)
	{
	}
}

[PublicApi(nameof(IExtensionHostChannel))]
public interface IExtensionHostChannel
{
	[PublicApi(nameof(ManifestLoaded))]
	event EventHandler<ManifestLoadedEventArgs> ManifestLoaded;

	[PublicApi(nameof(ReloadManifest))]
	Task<bool> ReloadManifest(string packageId);
}
