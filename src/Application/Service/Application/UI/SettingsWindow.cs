using System;
using System.Windows.Forms;

namespace Raid.Service.UI
{
    public partial class SettingsWindow : Form
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SettingsWindow_Load(object sender, EventArgs e)
        {
            autoUpdateCheckBox.Checked = AppConfiguration.AutomaticallyCheckForUpdates;
            runOnStartCheckBox.Checked = AppConfiguration.RunOnStartup;
            clickToStartCheckBox.Checked = AppConfiguration.ClickToStart;
            prereleaseBuildsCheckBox.Checked = AppConfiguration.InstallPrereleases;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            AppConfiguration.AutomaticallyCheckForUpdates = autoUpdateCheckBox.Checked;
            AppConfiguration.RunOnStartup = runOnStartCheckBox.Checked;
            AppConfiguration.ClickToStart = clickToStartCheckBox.Checked;
            AppConfiguration.InstallPrereleases = prereleaseBuildsCheckBox.Checked;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
        }
    }
}
