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

        public static PackageDescriptor FromAssembly(Assembly assembly)
        {
            ExtensionManifest manifest = ExtensionManifest.FromAssembly(assembly);
            return new(manifest.Id, manifest.DisplayName, manifest.Description, assembly);
        }

        public PackageDescriptor(string id, string name, string description, string location)
            => (Id, Name, Description, Location) = (id, name, description, location);

        public PackageDescriptor(string id, string name, string description, Assembly assembly)
            => (Id, Name, Description, Assembly) = (id, name, description, assembly);
    }
}
