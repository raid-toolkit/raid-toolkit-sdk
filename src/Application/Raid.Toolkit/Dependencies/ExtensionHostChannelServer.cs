using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.Common.API;

using System.Threading;

namespace Raid.Toolkit;

public class ExtensionHostChannelServer : ApiServer<IExtensionHostChannel>, IExtensionHostChannel, IHostedService
{
	private readonly IServerApplication ServerApplication;

	public event EventHandler<ManifestLoadedEventArgs>? ManifestLoaded;

	public ExtensionHostChannelServer(IServerApplication serverApplication, ILogger<ApiServer<IExtensionHostChannel>> logger)
		: base(logger)
	{
		ServerApplication = serverApplication;
	}

	public Task<bool> ReloadManifest(string packageId)
	{
		ManifestLoaded?.Invoke(this, new(packageId));
		// TODO: Implement menu reload from updated manifest
		return Task.FromResult(true);
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		ServerApplication.RegisterApiServer(this);
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}
