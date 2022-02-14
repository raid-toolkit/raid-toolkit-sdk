namespace Raid.Toolkit.Extensibility
{
    public class PackageContext : IPackageContext
    {
        public PackageDescriptor Descriptor { get; }

        public PackageContext(PackageDescriptor descriptor) => Descriptor = descriptor;
    }
}
