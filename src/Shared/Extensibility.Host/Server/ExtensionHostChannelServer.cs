using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.Common.API;

namespace Raid.Toolkit.Extensibility.Host.Server;

public class ExtensionHostChannelServer : ApiServer<IExtensionHostChannel>, IExtensionHostChannel, IHostedService
{
	private readonly IServerApplication? ServerApplication;

	public event EventHandler<ManifestLoadedEventArgs>? ManifestLoaded;

	public ExtensionHostChannelServer(IServiceProvider serviceProvider, ILogger<ApiServer<IExtensionHostChannel>> logger)
		: base(logger)
	{
		ServerApplication = serviceProvider.GetService<IServerApplication>();
	}

	public Task<bool> ReloadManifest(string packageId)
	{
		ManifestLoaded?.Invoke(this, new(packageId));
		// TODO: Implement menu reload from updated manifest
		return Task.FromResult(true);
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		ServerApplication?.RegisterApiServer(this);
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}
