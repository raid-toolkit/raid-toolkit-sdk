using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host
{
    public enum ExtensionState
    {
        None,
        Loaded,
        Activated,
        Error,
        Disabled,
        PendingUninstall,
    }
    public interface IExtensionManagement : IExtensionHost
    {
        public ExtensionState State { get; }
        public ExtensionBundle Bundle { get; }
    }
    public interface IExtensionHostController
    {
        IReadOnlyList<IExtensionManagement> GetExtensions();
        Task LoadExtensions();
        void ActivateExtensions();
        void ShowExtensionUI();
        void DeactivateExtensions();
        void UninstallPackage(string packageId);
        void DisablePackage(string packageId);
        void EnablePackage(string packageId);
        ExtensionHost GetExtensionPackageHost(string packageId);
    }
}
