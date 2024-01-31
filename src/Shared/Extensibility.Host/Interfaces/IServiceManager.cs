using System;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host;

public interface IServiceManager
{
	IDisposable AddService(IBackgroundService service);
	Task ProcessInstance(IGameInstance instance);
}
