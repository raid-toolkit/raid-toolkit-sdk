namespace Raid.Toolkit.Extensibility
{
    public interface IExtensionHost
    {
        void LoadExtensions();
        void ActivateExtensions();
        void InstallPackage(PackageDescriptor descriptor, bool activate);
        void UninstallPackage(PackageDescriptor descriptor);
    }
}
