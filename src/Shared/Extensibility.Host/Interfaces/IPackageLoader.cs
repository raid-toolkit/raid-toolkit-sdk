namespace Raid.Toolkit.Extensibility.Host
{
    public interface IPackageLoader
    {
        public IExtensionPackage LoadPackage(PackageDescriptor descriptor);
    }
}
