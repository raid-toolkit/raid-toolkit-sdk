using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.Injection
{
    public static class StructBaseExtensions
    {
        public static void CallMethod<T>(this T obj, ClientApi api, string methodName, params ArgumentValue[] args) where T : StructBase
        {
            api.CallMethod(obj, methodName, args);
        }
    }
}
