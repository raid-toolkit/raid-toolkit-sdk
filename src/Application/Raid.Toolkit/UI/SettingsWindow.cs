using System;
using System.Windows.Forms;
using Raid.Toolkit.Common;

namespace Raid.Toolkit.UI
{
    public partial class SettingsWindow : Form
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SettingsWindow_Load(object sender, EventArgs e)
        {
            autoUpdateCheckBox.Checked = RegistrySettings.AutomaticallyCheckForUpdates;
            runOnStartCheckBox.Checked = RegistrySettings.RunOnStartup;
            clickToStartCheckBox.Checked = RegistrySettings.ClickToStart;
            prereleaseBuildsCheckBox.Checked = RegistrySettings.InstallPrereleases;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            RegistrySettings.AutomaticallyCheckForUpdates = autoUpdateCheckBox.Checked;
            RegistrySettings.RunOnStartup = runOnStartCheckBox.Checked;
            RegistrySettings.ClickToStart = clickToStartCheckBox.Checked;
            RegistrySettings.InstallPrereleases = prereleaseBuildsCheckBox.Checked;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
        }
    }
}
