using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Raid.Toolkit.Common;

using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Raid.Toolkit.UI.WinUI
{
    public sealed partial class SettingsPanel : UserControl
    {
        public event EventHandler<EventArgs>? SettingsSaved;
        public event EventHandler<EventArgs>? SettingsDiscarded;

        public SettingsPanel()
        {
            InitializeComponent();
            AutoUpdate.IsChecked = RegistrySettings.AutomaticallyCheckForUpdates;
            RunOnStartup.IsChecked = RegistrySettings.RunOnStartup;
            ClickToFocus.IsChecked = RegistrySettings.ClickToStart;
            InstallPreRelease.IsChecked = RegistrySettings.InstallPrereleases;
            MinHeight = 250;
            MinWidth = 400;
        }

        private void OnSave(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            RegistrySettings.AutomaticallyCheckForUpdates = AutoUpdate.IsChecked == true;
            RegistrySettings.RunOnStartup = RunOnStartup.IsChecked == true;
            RegistrySettings.ClickToStart = ClickToFocus.IsChecked == true;
            RegistrySettings.InstallPrereleases = InstallPreRelease.IsChecked == true;
            RegistrySettings.FirstRun = false;
            DiscardButton.IsEnabled = true;
            SettingsSaved?.Invoke(this, new());
        }

        private void OnDiscard(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            SettingsDiscarded?.Invoke(this, new());
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DiscardButton.IsEnabled = !RegistrySettings.FirstRun;
        }
    }
}
