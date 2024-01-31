using Microsoft.Extensions.Hosting;

using Raid.Toolkit.Common.API;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Raid.Toolkit.Extensibility.Host;

public class WorkerApplication : IWorkerApplication, IHostedService
{
	private readonly ILogger Logger;
	public IPCApiClient Client { get; }

	public WorkerApplication(ILogger<IHostedService> logger)
	{
		Client = new(Constants.IPCPipeName);
		Logger = logger;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		CancellationTokenSource timedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		timedCancellation.CancelAfter(5000);
		try
		{
			await Client.ConnectAsync(timedCancellation.Token);
		}
		catch (OperationCanceledException ex)
		{
			Logger.LogError(ex, "Timed out connecting to main process");
			throw;
		}
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		Client.Disconnect();
		return Task.CompletedTask;
	}
}
