using Microsoft.Extensions.Hosting;

using Raid.Toolkit.Extensibility.Interfaces;
using Raid.Toolkit.Common.API;
using Raid.Toolkit.Common.API.Messages;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Raid.Toolkit.IPC;

namespace Raid.Toolkit.Extensibility.Implementations;

public class WorkerApplication : IWorkerApplication, IHostedService
{
	private static readonly ApiMessageSerializer Serializer = new();
	public IPCApiClient Client { get; }

	public WorkerApplication()
	{
		Client = new(Constants.IPCPipeName);
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		await Client.ConnectAsync(cancellationToken);
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		Client.Disconnect();
		return Task.CompletedTask;
	}
}
