using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Raid.Service
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> GetTypesAssignableTo<T>(this Assembly assembly)
        {
            return assembly.GetTypes().Where(type => !type.IsAbstract && type.IsAssignableTo(typeof(T)));
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