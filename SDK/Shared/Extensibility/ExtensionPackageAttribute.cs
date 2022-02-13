using System;
namespace Raid.Toolkit.Extensibility
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExtensionPackageAttribute : Attribute
    {
        public PackageDescriptor Descriptor { get; }

        public ExtensionPackageAttribute(string id, string name, string description)
            => Descriptor = new(id, name, description);
    }
}
