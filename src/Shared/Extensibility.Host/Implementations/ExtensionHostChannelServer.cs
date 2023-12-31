using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Raid.Toolkit.Common;
using Raid.Toolkit.Common.API;
using Raid.Toolkit.Extensibility.Implementations;
using Raid.Toolkit.Extensibility.Interfaces;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host
{
	public class ExtensionHostChannelServer : ApiServer<IExtensionHostChannel>, IExtensionHostChannel, IHostedService
	{
		private readonly IServiceProvider ServiceProvider;
		private readonly IServerApplication ServerApplication;

		public ExtensionHostChannelServer(IServiceProvider serviceProvider, IServerApplication serverApplication, ILogger<ApiServer<IExtensionHostChannel>> logger)
			: base(logger)
		{
			ServiceProvider = serviceProvider;
			ServerApplication = serverApplication;
		}

		public Task<bool> ReloadManifest(string packageId)
		{
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
}
