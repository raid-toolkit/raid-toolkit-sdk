using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> GetTypesAssignableTo<T>(this Assembly assembly)
        {
            return assembly.GetTypes().Where(type => !type.IsAbstract && type.IsAssignableTo(typeof(T)));
        }

        public static IEnumerable<Type> GetTypesWithAttribute<T>(this Assembly assembly) where T : Attribute
        {
            return assembly.GetTypes().Where(type => !type.IsAbstract).Select(type => new Tuple<Type, T>(type, type.GetCustomAttribute<T>())).Where(tuple => tuple.Item2 != null).Select(tuple => tuple.Item1);
        }

        public static IEnumerable<U> GetAttributes<T, U>(this Assembly assembly, Func<T, Type, U> valueSelector) where T : Attribute
        {
            return assembly.GetTypes().Where(type => !type.IsAbstract)
                .Select(type => new Tuple<Type, T>(type, type.GetCustomAttribute<T>()))
                .Where(tuple => tuple.Item2 != null).Select(tuple => valueSelector(tuple.Item2, tuple.Item1));
        }

        public static IEnumerable<T> ConstructTypesAssignableTo<T>(this Assembly assembly, params object[] arguments)
        {
            return assembly.GetTypes().Where(type => !type.IsAbstract && type.IsAssignableTo(typeof(T))).ConstructEach<T>(arguments);
        }

        public static IEnumerable<TBase> ConstructEach<TBase>(this IEnumerable<Type> types, params object[] arguments)
        {
            return types.Select(type => (TBase)Activator.CreateInstance(type, arguments));
        }
    }
}
