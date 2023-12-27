using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using WinUIEx;

namespace Raid.Toolkit.UI.WinUI.Base
{
    public abstract class RTKWindow : WindowEx
    {
        private static readonly EmbeddedIconId AppIcon = new(0);

        protected RTKWindow()
        {
            this.SetIcon(AppIcon.Value);
        }

        protected void CenterWindowInMonitor()
        {
			this.CenterWindowInMonitor();
        }
    }
}
