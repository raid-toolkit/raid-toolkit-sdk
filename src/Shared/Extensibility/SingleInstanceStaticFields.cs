using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.Extensibility
{
    [Size(16)]
    public struct SingleInstanceStaticFields<T>
    {
        [Offset(8)]
#pragma warning disable 649
        public T Instance;
#pragma warning restore 649
    }
}
