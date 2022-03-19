using System;
using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.Extensibility
{
    public interface IGameInstance : IDisposable
    {
        int Token { get; }
        string Id { get; }
        Il2CsRuntimeContext Runtime { get; }
        PropertyBag Properties { get; }
    }
}
