namespace Raid.Toolkit.Extensibility
{
    public interface IPackageLoader
    {
        public IExtensionPackage LoadPackage(PackageDescriptor descriptor);
    }
}
