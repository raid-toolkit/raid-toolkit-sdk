using System;

namespace Raid.Toolkit.Extensibility.Host
{
    public interface IServiceManager
    {
        IDisposable AddService(IBackgroundService service);
        void ProcessInstance(IGameInstance instance);
    }
}