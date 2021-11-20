using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Raid.Service.UI
{
    public partial class InstallWindow : Form
    {
        public InstallWindow()
        {
            InitializeComponent();
        }

        private void InstallWindow_Load(object sender, EventArgs e)
        {
            installationDirectory.Text = AppConfiguration.InstallationPath;
            installFolderDialog.SelectedPath = installationDirectory.Text;
        }

        private void selectInstallFolderButton_Click(object sender, EventArgs e)
        {
            if (installFolderDialog.ShowDialog() == DialogResult.OK)
            {
                installationDirectory.Text = installFolderDialog.SelectedPath;
            }
        }

        private void installButton_Click(object sender, EventArgs e)
        {
            try
            {
                var existingProcesses = Process.GetProcessesByName("Raid.Service").Where(proc => proc.Id != Environment.ProcessId).ToList();
                if (existingProcesses.Count > 0)
                {
                    var result = MessageBox.Show(
                        this,
                        "Raid Toolkit is currently running on this system, and must be closed before proceeding. Would you like to close it now?",
                        "Error",
                        MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        foreach (var proc in existingProcesses)
                            proc.Kill();
                    }
                    else
                    {
                        return;
                    }
                }

                string installDir = installationDirectory.Text;
                if (!Directory.Exists(installDir))
                    Directory.CreateDirectory(installDir);

                string newExecutablePath = Path.Combine(installDir, AppConfiguration.ExecutableName);
                if (AppConfiguration.ExecutablePath.ToLowerInvariant() != newExecutablePath.ToLowerInvariant())
                {
                    File.Copy(AppConfiguration.ExecutablePath, newExecutablePath, true);
                }
                AppConfiguration.InstallationPath = installDir;
                AppConfiguration.RunOnStartup = runOnStartCheckBox.Checked;
                AppConfiguration.AutomaticallyCheckForUpdates = autoUpdateCheckBox.Checked;
                AppConfiguration.UpdateStartMenuShortcut(createShortcutCheckBox.Checked);
                AppConfiguration.RegisterProtocol(true);
                AppConfiguration.IsInstalled = true;
                ProcessStartInfo psi = new(newExecutablePath);
                psi.ArgumentList.Add("--wait");
                psi.ArgumentList.Add("30000");
                Process.Start(psi);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"An error occurred:\n{ex.Message}", "Error", MessageBoxButtons.OK);
            }
        }
    }
}
