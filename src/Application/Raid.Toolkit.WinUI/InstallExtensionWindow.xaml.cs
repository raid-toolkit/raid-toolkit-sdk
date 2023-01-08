using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.Extensibility.Interfaces;
using Raid.Toolkit.UI.WinUI.Base;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

using Windows.UI.Popups;

using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Raid.Toolkit.UI.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InstallExtensionWindow : RTKWindow
    {
        public ExtensionBundle Bundle { get; }
        private readonly IPackageManager PackageManager;
        private readonly TaskCompletionSource<bool> DialogCompletionSource = new();

        public InstallExtensionWindow(IPackageManager packageManager, ExtensionBundle bundle)
        {
            PackageManager = packageManager;
            Bundle = bundle;
            InitializeComponent();

            DisplayName.Text = bundle.Manifest.DisplayName;
            Description.Text = bundle.Manifest.Description;

            CenterWindowInMonitor();
        }

        public Task<bool> RequestPermission()
        {
            Closed += (_, _) =>
            {
                DialogCompletionSource.TrySetResult(false);
            };
            Activate();
            return DialogCompletionSource.Task;
        }

        private void InstallButton_Click(object _, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            DialogCompletionSource.TrySetResult(true);
            Close();
        }

        private void CancelButton_Click(object _, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            DialogCompletionSource.TrySetResult(false);
            Close();
        }

        private void TrustButton_Click(object _, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            InstallButton.IsEnabled = true;
            MinHeight -= SecurityWarningBar.ActualHeight;
            Height -= SecurityWarningBar.ActualHeight;
            SecurityWarningBar.IsOpen = false;
        }
    }
}
