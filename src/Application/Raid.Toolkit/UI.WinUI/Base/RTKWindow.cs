using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using WinUIEx;
using static Windows.Win32.PInvoke;

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
            HWND hwnd = new(this.GetWindowHandle());
            RECT windowMonitorRectToAdjust = new RECT(AppWindow.Position.X, AppWindow.Position.Y, (int)Width, (int)Height);
            ClipOrCenterRectToMonitorWin32(ref windowMonitorRectToAdjust);
            SetWindowPos(hwnd, default, windowMonitorRectToAdjust.left,
                windowMonitorRectToAdjust.top, 0, 0,
                SET_WINDOW_POS_FLAGS.SWP_NOSIZE |
                SET_WINDOW_POS_FLAGS.SWP_NOZORDER |
                SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);
        }

        private void ClipOrCenterRectToMonitorWin32(ref RECT adjustedWindowRect)
        {
            MONITORINFO mi = new MONITORINFO();
            mi.cbSize = (uint)Marshal.SizeOf<MONITORINFO>();
            GetMonitorInfo(MonitorFromRect(adjustedWindowRect, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST), ref mi);

            RECT rcWork = mi.rcWork;
            int w = adjustedWindowRect.right - adjustedWindowRect.left;
            int h = adjustedWindowRect.bottom - adjustedWindowRect.top;

            adjustedWindowRect.left = rcWork.left + (rcWork.right - rcWork.left - w) / 2;
            adjustedWindowRect.top = rcWork.top + (rcWork.bottom - rcWork.top - h) / 2;
            adjustedWindowRect.right = adjustedWindowRect.left + w;
            adjustedWindowRect.bottom = adjustedWindowRect.top + h;
        }
    }
}
