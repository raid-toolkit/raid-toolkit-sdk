using System;
using System.Reflection;

namespace Raid.DataServices
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DataTypeAttribute : Attribute
    {
        public string Key { get; }

        public string Version { get => StructuredVersion.ToString(); set => StructuredVersion = System.Version.Parse(value); }
        public Version StructuredVersion = System.Version.Parse("1.0");

        public DataTypeAttribute(string key)
            : base()
        {
            Key = key;
        }

        public static string GetKey(Type type)
        {
            return type.GetCustomAttribute<DataTypeAttribute>().Key;
        }

        public static Version GetVersion(Type type)
        {
            return type.GetCustomAttribute<DataTypeAttribute>().StructuredVersion;
        }
    }
}