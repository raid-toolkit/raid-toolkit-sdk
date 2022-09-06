using System;
using System.Windows.Forms;
using Microsoft.UI.Xaml;
using Raid.Toolkit.WinUI.Base;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Bound property")]
#pragma warning disable CS0436 // Type conflicts with imported type
        public string VersionString => ThisAssembly.AssemblyFileVersion;
#pragma warning restore CS0436 // Type conflicts with imported type

        private bool IsDisposed;
        private EmbeddedIconId AppIcon = new(0);

        private readonly ContextMenuStrip contextMenu;
        private NotifyIcon notifyIcon;

        public MainWindow() : base()
        {
            InitializeComponent();
            contextMenu = new();
            _ = contextMenu.Items.Add(new ToolStripMenuItem("Exit", null, (_, _) => Close()));
            notifyIcon = new()
            {
                Text = "Raid Toolkit",
                Icon = FormsResources.AppIcon,
                Visible = true,
                ContextMenuStrip = contextMenu
            };
            notifyIcon.Click += NotifyIcon_Click;
            AppWindow.Closing += AppWindow_Closing;
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            _ = this.Show();
        }

        private void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
        {
            _ = this.Hide();
            args.Cancel = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _ = this.Hide();
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
