using System;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility;

public interface IAppUI : IAppDispatcher, IDisposable
{
    void Run();

    void ShowMain();
    void ShowSettings();
    void ShowExtensionManager();
}
