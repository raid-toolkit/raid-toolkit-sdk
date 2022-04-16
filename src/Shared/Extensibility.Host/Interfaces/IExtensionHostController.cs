using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host
{
    public interface IExtensionHostController : IExtensionHost
    {
        Task LoadExtensions();
        void ActivateExtensions();
        void ShowExtensionUI();
        void DeactivateExtensions();
        void InstallPackage(ExtensionBundle descriptor, bool activate);
        void UninstallPackage(string packageId);
    }
}
