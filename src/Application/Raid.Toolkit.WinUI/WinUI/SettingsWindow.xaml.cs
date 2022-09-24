using Raid.Toolkit.Common;
using Raid.Toolkit.WinUI.Base;

using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Raid.Toolkit.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsWindow : RTKWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
            AutoUpdate.IsChecked = RegistrySettings.AutomaticallyCheckForUpdates;
            RunOnStartup.IsChecked = RegistrySettings.RunOnStartup;
            ClickToFocus.IsChecked = RegistrySettings.ClickToStart;
            InstallPreRelease.IsChecked = RegistrySettings.InstallPrereleases;

            this.CenterOnScreen(400, 250);
            Backdrop = new MicaSystemBackdrop();
            MinHeight = 250;
            MinWidth = 400;
        }

        private void OnSave(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            RegistrySettings.AutomaticallyCheckForUpdates = AutoUpdate.IsChecked == true;
            RegistrySettings.RunOnStartup = RunOnStartup.IsChecked == true;
            RegistrySettings.ClickToStart = ClickToFocus.IsChecked == true;
            RegistrySettings.InstallPrereleases = InstallPreRelease.IsChecked == true;
            Close();
        }

        private void OnCancel(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Close();
        }
    }
}
