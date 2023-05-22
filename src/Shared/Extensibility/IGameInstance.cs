using System;
using System.Diagnostics;
using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.Extensibility
{
    public interface IGameInstance : IDisposable
    {
        int Token { get; }
        string Id { get; }
        string AvatarUrl { get; }
        Il2CsRuntimeContext Runtime { get; }
        PropertyBag Properties { get; }
        void InitializeOrThrow(Process proc);
    }
}
