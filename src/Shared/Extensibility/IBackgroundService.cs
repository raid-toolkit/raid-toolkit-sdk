using System;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility;

public interface IBackgroundService
{
	TimeSpan PollInterval { get; }
	Task Tick(IGameInstance instance);
}
