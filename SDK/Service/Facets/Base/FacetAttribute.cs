using System;
using System.Reflection;

namespace Raid.Service
{
    public class FacetAttribute : Attribute
    {
        public string Name { get; }
        public string Version { get => StructuredVersion.ToString(); set => StructuredVersion = System.Version.Parse(value); }
        public Version StructuredVersion = System.Version.Parse("1.0");

        public FacetAttribute(string name) => Name = name;

        public static string GetName(Type type)
        {
            return type.GetCustomAttribute<FacetAttribute>().Name;
        }

        public static Version GetVersion(Type type)
        {
            return type.GetCustomAttribute<FacetAttribute>().StructuredVersion;
        }
    }
}
