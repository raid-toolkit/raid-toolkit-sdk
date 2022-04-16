namespace Raid.Toolkit.Extensibility.Host
{
    public interface IPackageLoader
    {
        public IExtensionPackage LoadPackage(ExtensionBundle bundle);
    }
}
