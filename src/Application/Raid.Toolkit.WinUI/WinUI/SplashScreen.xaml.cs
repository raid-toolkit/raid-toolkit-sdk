using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Raid.Toolkit.WinUI.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Raid.Toolkit.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SplashScreen : RTKWindow
    {
        public SplashScreen()
        {
            this.InitializeComponent();
            
            this.SetTitleBarBackgroundColors(Microsoft.UI.Colors.Purple);
            
            IsShownInSwitchers = true;
            IsMinimizable = false;
            IsMaximizable = false;
            IsResizable = false;
            this.SetWindowPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.Overlapped);
            this.CenterOnScreen(400, 550);
#pragma warning disable CS0436 // Type conflicts with imported type
            this.VersionRTK.Text = ThisAssembly.AssemblyFileVersion;
#pragma warning restore CS0436 // Type conflicts with imported type
        }
    }
}
