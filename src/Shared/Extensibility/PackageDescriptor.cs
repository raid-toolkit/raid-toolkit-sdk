namespace Raid.Toolkit.Extensibility
{
    public class PackageDescriptor
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }

        public PackageDescriptor(string id, string name, string description)
            => (Id, Name, Description) = (id, name, description);
    }
}
