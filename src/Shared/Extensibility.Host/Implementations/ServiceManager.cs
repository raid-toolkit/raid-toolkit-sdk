using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Raid.Toolkit.Extensibility.Host
{
    public class ServiceManager : IServiceManager
    {
        private class ServiceState
        {
            public IBackgroundService service;
            public DateTime nextTick;
        }
        private readonly List<ServiceState> BackgroundServices = new();
        private readonly ILogger<ServiceManager> Logger;
        public ServiceManager(ILogger<ServiceManager> logger)
        {
            Logger = logger;
        }

        public IDisposable AddService(IBackgroundService service)
        {
            ServiceState serviceState = new()
            {
                service = service,
                nextTick = DateTime.MinValue
            };
            BackgroundServices.Add(serviceState);
            return new HostResourceHandle(() => BackgroundServices.Remove(serviceState));
        }

        public async Task ProcessInstance(IGameInstance instance)
        {
            foreach (var service in BackgroundServices)
            {
                if (service.nextTick < DateTime.UtcNow)
                {
                    try
                    {
                        // don't run again until current tick finishes
                        service.nextTick = DateTime.MaxValue;
                        await service.service.Tick(instance);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, $"Failure in background service {service.GetType().FullName}");
                    }
                    service.nextTick = DateTime.UtcNow.Add(service.service.PollInterval);
                }
            }
        }
    }
}
