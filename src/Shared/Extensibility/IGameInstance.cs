using System;
using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.Extensibility
{
    public interface IGameInstance : IDisposable
    {
        public int Token { get; }
        public string Id { get; }
        public Il2CsRuntimeContext Runtime { get; }
    }
}
