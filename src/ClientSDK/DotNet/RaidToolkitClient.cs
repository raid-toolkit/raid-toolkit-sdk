using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Raid.Toolkit.Common;
using Raid.Toolkit.DataModel;

namespace Raid.Client
{
    public class RaidToolkitClient : RaidToolkitClientBase
    {
        public IAccountApi AccountApi => MakeApi<AccountApi>();
        public IStaticDataApi StaticDataApi => MakeApi<StaticDataApi>();
        public IRealtimeApi RealtimeApi => MakeApi<RealtimeApi>();

        public static async Task EnsureInstalled()
        {
            if (!RegistrySettings.IsInstalled)
            {
                using var form = new Form { TopMost = true };
                var response = MessageBox.Show(
                    form,
                    "Raid Toolkit is required to be installed to access game data, would you like to download and install it now?",
                    "Installation required",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                    );
                if (response != DialogResult.Yes)
                {
                    throw new NotSupportedException("Raid Toolkit must be installed");
                }
                try
                {
                    await InstallRTK();
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(form, $"An error ocurred\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static async Task InstallRTK()
        {
            GitHub.Updater updater = new();
            GitHub.Schema.Release release = await updater.GetLatestRelease();
            if (release == null)
            {
                throw new FileNotFoundException("Could not find the latest release");
            }

            string tempFile = Path.Combine(Path.GetTempPath(), "RaidToolkitSetup.exe");
            using (var stream = await updater.DownloadSetup(release, null))
            {
                using Stream newFile = File.Create(tempFile);
                stream.CopyTo(newFile);
            }
            Process proc = Process.Start(tempFile);
            // NET 472 doesn't support WaitForExitAsync
            proc.WaitForExit();
            // await proc.WaitForExitAsync();
        }
    }
}
