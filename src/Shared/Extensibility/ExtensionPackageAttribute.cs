using System;

namespace Raid.Toolkit.Extensibility
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ExtensionPackageAttribute : Attribute
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }

        public ExtensionPackageAttribute(string id, string name, string description)
            => (Id, Name, Description) = (id, name, description);
    }
}
