namespace Raid.Toolkit.Extensibility.Host
{
    public interface IExtensionHostController : IExtensionHost
    {
        void LoadExtensions();
        void ActivateExtensions();
        void InstallPackage(PackageDescriptor descriptor, bool activate);
        void UninstallPackage(PackageDescriptor descriptor);
    }
}
