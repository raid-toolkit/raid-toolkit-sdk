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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Forms;
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
    public sealed partial class MainWindow : RTKWindow, IDisposable
    {
        public string VersionString => ThisAssembly.AssemblyFileVersion;

        private bool IsDisposed;
        private EmbeddedIconId AppIcon = new(0);
        private NotifyIcon notifyIcon;
        public MainWindow() : base()
        {
            this.InitializeComponent();
            notifyIcon = new()
            {
                Text = "Raid Toolkit",
                Icon = FormsResources.AppIcon,
                Visible = true
            };
            notifyIcon.Click += NotifyIcon_Click;
            //this.CenterOnScreen(250, 400);
            //this.SetIcon(AppIcon.Value);
            AppWindow.Closing += AppWindow_Closing;
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
        {
            this.Hide();
            args.Cancel = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                AppIcon?.Dispose();
                AppIcon = null;

                notifyIcon?.Dispose();
                notifyIcon = null;

                // TODO: set large fields to null
                IsDisposed = true;
            }
        }

        ~MainWindow()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
