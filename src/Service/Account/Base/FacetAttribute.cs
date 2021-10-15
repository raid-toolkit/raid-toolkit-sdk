using System;
using System.Reflection;

namespace Raid.Service
{
    public class FacetAttribute : Attribute
    {
        public string Name { get; }
        public FacetAttribute(string name) => Name = name;

        public static string GetName(Type type)
        {
            return type.GetCustomAttribute<FacetAttribute>().Name;
        }
    }
}