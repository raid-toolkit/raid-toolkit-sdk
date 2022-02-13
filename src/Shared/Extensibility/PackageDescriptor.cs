namespace Raid.Toolkit.Extensibility
{
    public sealed class PackageDescriptor
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Location { get; }

        public static PackageDescriptor FromAttribute(ExtensionPackageAttribute attribute, string location)
        {
            return new(attribute.Id, attribute.Name, attribute.Description, location);
        }

        public PackageDescriptor(string id, string name, string description, string location)
            => (Id, Name, Description, Location) = (id, name, description, location);
    }
}
