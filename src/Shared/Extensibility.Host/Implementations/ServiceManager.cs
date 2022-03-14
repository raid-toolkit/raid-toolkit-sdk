using System;
using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility.Host
{
    public class ServiceManager : IServiceManager
    {
        private class ServiceState
        {
            public IBackgroundService service;
            public DateTime nextTick;
        }
        private List<ServiceState> BackgroundServices = new();

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

        public void ProcessInstance(IGameInstance instance)
        {
            foreach (var service in BackgroundServices)
            {
                if (service.nextTick < DateTime.UtcNow)
                {
                    service.service.Tick(instance);
                    service.nextTick = DateTime.UtcNow.Add(service.service.PollInterval);
                }
            }
        }
    }
}