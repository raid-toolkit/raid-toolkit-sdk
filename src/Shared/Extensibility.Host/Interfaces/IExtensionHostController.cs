using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
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
        public Regex[] GetIncludeTypes();
        public Task Load();
        public void Activate();
        public void Deactivate();
        public void Install();
        public void Uninstall();
        public void ShowUI();
        public int HandleRequest(Uri requestUri);
    }
    public interface IExtensionHostController
    {
        IReadOnlyList<IExtensionManagement> GetExtensions();
        bool TryGetExtension(string packageId, [NotNullWhen(true)] out IExtensionManagement? extension);
        Task LoadExtensions();
        void ActivateExtensions();
        void ShowExtensionUI();
        void DeactivateExtensions();
        void UninstallPackage(string packageId);
        void DisablePackage(string packageId);
        void EnablePackage(string packageId);
    }
}
