using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Raid.Toolkit.Common;

namespace Raid.Toolkit.UI
{
    public partial class InstallWindow : Form
    {
        public InstallWindow()
        {
            InitializeComponent();
        }

        private void InstallWindow_Load(object sender, EventArgs e)
        {
            installationDirectory.Text = RegistrySettings.InstallationPath;
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
                var existingProcesses =
                    Process.GetProcessesByName("Raid.Service")
                    .Concat(Process.GetProcessesByName("Raid.Toolkit"))
                    .Where(proc => proc.Id != Environment.ProcessId).ToList();
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
                    _ = Directory.CreateDirectory(installDir);

                string exeName = AppHost.ExecutableName;
                string newExePath = Path.Combine(installDir, exeName);
                if (AppHost.ExecutablePath.ToLowerInvariant() != newExePath.ToLowerInvariant())
                {
                    File.Copy(AppHost.ExecutablePath, newExePath, true);
                }
                RegistrySettings.InstallationPath = installDir;
                RegistrySettings.RunOnStartup = runOnStartCheckBox.Checked;
                RegistrySettings.AutomaticallyCheckForUpdates = autoUpdateCheckBox.Checked;
                RegistrySettings.UpdateStartMenuShortcut(createShortcutCheckBox.Checked);
                RegistrySettings.RegisterProtocol(true);
                RegistrySettings.IsInstalled = true;
                AppHost.EnsureFileAssociations(newExePath);
                ProcessStartInfo psi = new(newExePath);
                psi.ArgumentList.Add("--wait");
                psi.ArgumentList.Add("30000");
                _ = Process.Start(psi);
                Close();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(this, $"An error occurred:\n{ex.Message}", "Error", MessageBoxButtons.OK);
            }
        }
    }
}
