using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.HostInterfaces
{
    public interface IExtensionHostController : IExtensionHost
    {
        Task LoadExtensions();
        void ActivateExtensions();
        void InstallPackage(PackageDescriptor descriptor, bool activate);
        void UninstallPackage(PackageDescriptor descriptor);
    }
}
