using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Raid.Toolkit.Extensibility.Host.Services;

public class ServiceExecutor : PollingBackgroundService
{
	private readonly static TimeSpan kPollInterval = new(0, 0, 0, 0, 50);
	private protected override TimeSpan PollInterval => kPollInterval;
	private readonly IServiceManager ServiceManager;
	private readonly IGameInstanceManager InstanceManager;

	public ServiceExecutor(
		ILogger<FileStorageService> logger,
		IServiceManager serviceManager,
		IGameInstanceManager instanceManager) : base(logger)
	{
		ServiceManager = serviceManager;
		InstanceManager = instanceManager;
	}

	protected override async Task ExecuteOnceAsync(CancellationToken token)
	{
		foreach (var instance in InstanceManager.Instances)
		{
			try
			{
				await ServiceManager.ProcessInstance(instance);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Exception thrown in service");
			}
		}
	}
}
