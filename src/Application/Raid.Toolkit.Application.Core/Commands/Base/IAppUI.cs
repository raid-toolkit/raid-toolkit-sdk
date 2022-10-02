using System;

using Raid.Toolkit.Extensibility;

namespace Raid.Toolkit.Application.Core.Commands.Base
{
    public interface IAppUI : IDisposable
    {
        SynchronizationContext? SynchronizationContext { get; }
        void Run();
        void Dispatch(Action task);

        bool? ShowExtensionInstaller(ExtensionBundle bundleToInstall);
        void ShowInstallUI();
        void ShowMain();
        void ShowSettings();
        void ShowErrors();
        void ShowExtensionManager();
        void ShowNotification(string title, string description, System.Windows.Forms.ToolTipIcon icon, int timeoutMs, Action? onActivate = null);
    }
}
