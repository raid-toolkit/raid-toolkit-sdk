using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.WinUI.Base
{
    /// <summary>
    ///  Specifies the initial position of a window.
    /// </summary>
    public enum WindowStartPosition
    {
        /// <summary>
        ///  The window is centered on the current display, and has the dimensions
        ///  specified in the window's size.
        /// </summary>
        CenterScreen = 1,

        /// <summary>
        ///  The window is positioned at the Windows default location and has the
        ///  dimensions specified in the window's size.
        /// </summary>
        WindowsDefaultLocation = 2,

        /// <summary>
        ///  The window is centered within the bounds of its parent window.
        /// </summary>
        CenterParent = 4
    }
}
