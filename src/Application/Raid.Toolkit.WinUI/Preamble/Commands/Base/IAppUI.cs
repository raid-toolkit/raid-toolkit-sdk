using System;
using Raid.Toolkit.Extensibility;

namespace Raid.Toolkit.App.Tasks.Base
{
    public interface IAppUI
    {
        bool? ShowExtensionInstaller(ExtensionBundle bundleToInstall);
        void ShowInstallUI();
        void ShowMain();
        void ShowSettings();
        void ShowErrors();
        void ShowExtensionManager();
        void ShowNotification(string title, string description, System.Windows.Forms.ToolTipIcon icon, int timeoutMs, Action? onActivate = null);
    }
}
