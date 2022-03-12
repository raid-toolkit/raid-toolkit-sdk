using System.Reflection;

namespace Raid.Toolkit.Extensibility
{
    public sealed class PackageDescriptor
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Location { get; }
        public Assembly Assembly { get; }

        public static PackageDescriptor FromAttribute(ExtensionPackageAttribute attribute, string location)
        {
            return new(attribute.Id, attribute.Name, attribute.Description, location);
        }

        public static PackageDescriptor FromAttribute(ExtensionPackageAttribute attribute, Assembly assembly)
        {
            return new(attribute.Id, attribute.Name, attribute.Description, assembly);
        }

        public PackageDescriptor(string id, string name, string description, string location)
            => (Id, Name, Description, Location) = (id, name, description, location);

        public PackageDescriptor(string id, string name, string description, Assembly assembly)
            => (Id, Name, Description, Assembly) = (id, name, description, assembly);
    }
}
