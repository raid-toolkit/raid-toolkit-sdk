using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Raid.Toolkit.Extensibility.Host;

public class ServiceManager : IServiceManager
{
	private class ServiceState
	{
		public ServiceState(IBackgroundService service)
		{
			Service = service;
			NextTickByInstanceToken = new();
		}
		public IBackgroundService Service;
		public Dictionary<int, DateTime> NextTickByInstanceToken;
	}
	private readonly List<ServiceState> BackgroundServices = new();
	private readonly ILogger<ServiceManager> Logger;
	public ServiceManager(ILogger<ServiceManager> logger)
	{
		Logger = logger;
	}

	public IDisposable AddService(IBackgroundService service)
	{
		ServiceState serviceState = new(service);
		BackgroundServices.Add(serviceState);
		return new HostResourceHandle(() => BackgroundServices.Remove(serviceState));
	}

	public async Task ProcessInstance(IGameInstance instance)
	{
		foreach (var service in BackgroundServices)
		{
			if (!service.NextTickByInstanceToken.TryGetValue(instance.Token, out DateTime nextTick) || nextTick < DateTime.UtcNow)
			{
				try
				{
					// don't run again until current tick finishes
					service.NextTickByInstanceToken[instance.Token] = DateTime.MaxValue;
					await service.Service.Tick(instance);
				}
				catch (Exception ex)
				{
					Logger.LogError(ex, "Failure in background service {typeName}", service.GetType().FullName);
				}
				service.NextTickByInstanceToken[instance.Token] = DateTime.UtcNow.Add(service.Service.PollInterval);
			}
		}
	}
}
