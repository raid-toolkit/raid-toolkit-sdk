using System;
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
    public interface IManagedPackage : IExtensionHost
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
		public void SaveManifest(PackageManifest manifest);
    }
    public interface IExtensionManager
    {
        bool TryGetExtension(string packageId, [NotNullWhen(true)] out IManagedPackage? extension);
        Task LoadExtensions();
        void DisablePackage(string packageId);
        void EnablePackage(string packageId);
    }
}
