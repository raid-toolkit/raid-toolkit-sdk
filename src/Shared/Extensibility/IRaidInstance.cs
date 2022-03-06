using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.Extensibility
{
    public interface IRaidInstance
    {
        public string Id { get; }
        public Il2CsRuntimeContext Runtime { get; }
    }
}
