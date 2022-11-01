using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using GitHub;
using GitHub.Schema;

using Newtonsoft.Json;

using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility;

using Path = System.IO.Path;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private readonly Updater Updater;
        private Release? LatestRelease;
        private readonly List<ExtensionManifest> UnsupportedExtensions = new();

        public MainWindow()
        {
            InitializeComponent();

            // first run experience not required if they previously used RTK
            if (RegistrySettings.IsInstalled)
            {
                RegistrySettings.FirstRun = false;
            }

            Updater = new Updater()
            {
                InstallPrereleases = RegistrySettings.InstallPrereleases
            };
            installPrereleaseCheckBox.IsChecked = RegistrySettings.InstallPrereleases;
        }

        private async Task RefreshRelease()
        {
            LatestRelease = null;
            progressBar.IsIndeterminate = true;
            updateButton.IsEnabled = false;
            installPrereleaseCheckBox.IsEnabled = false;
            latestReleaseCheck.Foreground = new SolidColorBrush(Colors.Silver);
            latestReleaseCheck.Status = CheckStatus.Running;
            try
            {
                LatestRelease = await Updater.GetLatestRelease();
                if (LatestRelease == null)
                {
                    latestReleaseCheck.DisplayName = $"Latest release: <unknown>";
                    latestReleaseCheck.Foreground = new SolidColorBrush(Colors.Red);
                    latestReleaseCheck.Status = CheckStatus.Fail;
                    return;
                }
                if (!Updater.IsValidRelease(LatestRelease))
                {
                    latestReleaseCheck.DisplayName = $"Latest release: {LatestRelease.TagName}\nInstaller not found!";
                    latestReleaseCheck.Foreground = new SolidColorBrush(Colors.Red);
                    latestReleaseCheck.Status = CheckStatus.Fail;
                }
                else
                {
                    latestReleaseCheck.DisplayName = $"Latest release: {LatestRelease.TagName}";
                    latestReleaseCheck.Foreground = new SolidColorBrush(Colors.LimeGreen);
                    latestReleaseCheck.Status = CheckStatus.Success;
                    updateButton.IsEnabled = true;
                }
            }
            finally
            {
                installPrereleaseCheckBox.IsEnabled = true;
                progressBar.IsIndeterminate = false;
            }
        }

        private async void installPrereleaseCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Updater.InstallPrereleases = installPrereleaseCheckBox.IsChecked;
            await RefreshRelease();
        }

        private async void updateButton_Click(object sender, RoutedEventArgs e)
        {
            if (LatestRelease == null)
                return;
            try
            {
                updateButton.IsEnabled = false;
                Progress<float> progress = new(value => progressBar.Value = value);
                string installerPath = Path.Combine(Path.GetTempPath(), "RaidToolkitSetup.exe");
                using (Stream installer = await Updater.DownloadSetup(LatestRelease, progress))
                using (FileStream fileStream = File.Create(installerPath))
                {
                    _ = installer.Seek(0, SeekOrigin.Begin);
                    installer.CopyTo(fileStream);
                    fileStream.Close();
                }
                string thisProcessPath = Process.GetCurrentProcess().MainModule!.FileName!;
                string thisProcessDir = Path.GetDirectoryName(thisProcessPath)!;
                string thisProcessRenamed = Path.Combine(thisProcessDir, "UpgradeLauncher.exe");
                if (File.Exists(thisProcessRenamed))
                    File.Delete(thisProcessRenamed);
                File.Move(thisProcessPath, thisProcessRenamed);
                _ = Process.Start(installerPath);
                Close();
            }
            catch (Exception ex)
            {
                latestReleaseCheck.Status = CheckStatus.Fail;
                latestReleaseCheck.DisplayName = ex.Message;
                latestReleaseCheck.Foreground = new SolidColorBrush(Colors.Red);
                updateButton.IsEnabled = true;
            }
        }

        private void OperatingSystem_Check(object sender, InstallCheckEventArgs e)
        {
            if (Environment.OSVersion.Version < new Version(10, 0, 18362))
            {
                e.Fail("Update to Windows 10 build 19041 or newer");
                return;
            }
            else
            {
                e.Succeed();
            }
        }

        private async void UnsupportedExtensions_Check(object sender, InstallCheckEventArgs e)
        {
            if (!RegistrySettings.IsInstalled)
            {
                e.Succeed();
                return;
            }
            string extensionsDir = Path.Combine(RegistrySettings.InstallationPath, "extensions");
            if (!Directory.Exists(extensionsDir))
            {
                e.Succeed();
                return;
            }
            string[] directories = Directory.GetDirectories(extensionsDir);
            foreach (string dir in directories)
            {
                string manifestPath = Path.Combine(dir, ".rtk.extension.json");
                if (!File.Exists(manifestPath))
                    continue;
                ExtensionManifest? manifest = JsonConvert.DeserializeObject<ExtensionManifest>(await File.ReadAllTextAsync(manifestPath));
                if (manifest == null)
                    continue;
                if (string.IsNullOrEmpty(manifest.RequireVersion) || (Version.TryParse(manifest.RequireVersion, out Version? version) && version < new Version(2, 0)))
                    UnsupportedExtensions.Add(manifest);
            }
            if (UnsupportedExtensions.Count == 0)
            {
                e.Succeed();
                return;
            }
            e.Fail("Unsupported extensions found");
        }

        private void UnsupportedExtensions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _ = MessageBox.Show(string.Join(' ', UnsupportedExtensions.Select(e => e.Id)), "Unsupported Extensions");
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await RefreshRelease();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
