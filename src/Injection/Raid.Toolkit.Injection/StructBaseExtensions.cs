using Il2CppToolkit.Runtime;
using System;

namespace Raid.Toolkit.Injection
{
    public static class StructBaseExtensions
    {
        public static void CallMethod<T>(this T obj, Type objType, ClientApi api, string methodName, params ArgumentValue[] args) where T : StructBase
        {
            api.CallMethod(obj, objType, methodName, args);
        }
        public static void CallMethod<T>(this T obj, ClientApi api, string methodName, params ArgumentValue[] args) where T : StructBase
        {
            api.CallMethod(obj, methodName, args);
        }
    }
}
