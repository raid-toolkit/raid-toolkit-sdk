using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.App.Tasks.Base
{
    public interface IAppUI
    {
        void HideRebuildUI();
        bool? ShowExtensionInstaller(ExtensionBundle bundleToInstall);
        void ShowInstallUI();
        void ShowMainWindow();
        void ShowRebuildUI(PlariumPlayAdapter.GameInfo gameInfo);
        void ShowUpdateNotification();
    }
}
