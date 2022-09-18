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
        bool? ShowExtensionInstaller(ExtensionBundle bundleToInstall);
        void ShowInstallUI();
        void ShowMain();
        void ShowUpdateNotification();
    }
}
