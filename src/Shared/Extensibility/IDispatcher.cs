using Microsoft.UI.Dispatching;

namespace Raid.Toolkit.Extensibility;

public interface IDispatcher
{
    DispatcherQueue UI { get; }
}
