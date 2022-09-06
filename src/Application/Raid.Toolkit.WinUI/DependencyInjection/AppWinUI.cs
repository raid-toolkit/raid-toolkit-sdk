using Raid.Toolkit.App.Tasks.Base;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Model;
using Raid.Toolkit.WinUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.DependencyInjection
{
    public class AppWinUI : IAppUI
    {
        private MainWindow? m_mainWindow;
        private RebuildWindow? m_rebuildWindow;

        public void ShowMainWindow()
        {
            RTKApplication.UIContext?.Post(_ =>
            {
                m_mainWindow = new();
                m_mainWindow.Activate();
            }, null);
        }

        public void ShowInstallUI()
        {
            throw new NotImplementedException();
        }

        public bool? ShowExtensionInstaller(ExtensionBundle bundleToInstall)
        {
            throw new NotImplementedException();
        }

        public void ShowRebuildUI(PlariumPlayAdapter.GameInfo gameInfo)
        {
            RTKApplication.UIContext?.Post(_ =>
            {
                m_rebuildWindow = new(gameInfo);
                m_rebuildWindow.Activate();
            }, null);
        }

        public void HideRebuildUI()
        {
            RTKApplication.UIContext?.Post(_ =>
            {
                m_rebuildWindow?.Close();
            }, null);
        }

        public void ShowUpdateNotification()
        {
            throw new NotImplementedException();
        }
    }
}
