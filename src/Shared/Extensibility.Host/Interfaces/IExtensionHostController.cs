using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host
{
    public interface IExtensionHostController : IExtensionHost
    {
        Task LoadExtensions();
        void ActivateExtensions();
        void ShowExtensionUI();
        void DeactivateExtensions();
        void InstallPackage(PackageDescriptor descriptor, bool activate);
        void UninstallPackage(PackageDescriptor descriptor);
    }
}
