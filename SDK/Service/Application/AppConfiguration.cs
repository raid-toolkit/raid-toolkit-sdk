using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Raid.Common;

namespace Raid.Service
{
    public static class AppConfiguration
    {
        private static readonly Lazy<IConfigurationRoot> _configuration = new(
            () => new ConfigurationBuilder()
                .SetBasePath(ExecutableDirectory)
                .AddJsonStream(RaidHost.GetEmbeddedSettings())
                .AddJsonFile("appsettings.json").Build()
            );
        public static IConfigurationRoot Configuration => _configuration.Value;
        public static readonly string ExecutablePath;
        public static readonly string ExecutableName;
        public static readonly string ExecutableDirectory;
        public static readonly string DefaultInstallationPath;
        public static readonly Version AppVersion = new(ThisAssembly.AssemblyFileVersion);

        // TODO: Reconcile this duplication with copy in Raid.Common.RegistrySettings

        public static bool IsInstalled
        {
            get => RegistrySettings.IsInstalled;
            set => Registry.CurrentUser.CreateSubKey(RegistrySettings.RTKHive).SetValue(RegistrySettings.IsInstalledKey, 1, RegistryValueKind.DWord);
        }

        public static bool ClickToStart
        {
            get => RegistrySettings.ClickToStart;
            set => Registry.CurrentUser.CreateSubKey(RegistrySettings.RTKHive).SetValue(RegistrySettings.ClickToStartKey, value ? 1 : 0, RegistryValueKind.DWord);
        }

        public static bool InstallPrereleases
        {
            get => RegistrySettings.InstallPrereleases;
            set => Registry.CurrentUser.CreateSubKey(RegistrySettings.RTKHive).SetValue(RegistrySettings.InstallPrereleasesKey, value ? 1 : 0, RegistryValueKind.DWord);
        }

        public static string InstalledExecutablePath => Path.Join(InstallationPath, ExecutableName);

        public static string InstallationPath
        {
            get
            {
                var hive = Registry.CurrentUser.OpenSubKey(RegistrySettings.RTKHive);
                return hive == null ? DefaultInstallationPath : (string)hive.GetValue(RegistrySettings.InstallFolderKey, DefaultInstallationPath);
            }
            set
            {
                var hive = Registry.CurrentUser.CreateSubKey(RegistrySettings.RTKHive);
                hive.SetValue(RegistrySettings.InstallFolderKey, value, RegistryValueKind.String);
            }
        }

        public static bool RunOnStartup
        {
            get => RegistrySettings.RunOnStartup;
            set
            {
                if (value)
                    Registry.CurrentUser.CreateSubKey(RegistrySettings.StartupHive).SetValue(RegistrySettings.StartupName, InstalledExecutablePath, RegistryValueKind.String);
                else
                    Registry.CurrentUser.CreateSubKey(RegistrySettings.StartupHive).DeleteValue(RegistrySettings.StartupName, false);
            }
        }

        public static bool AutomaticallyCheckForUpdates
        {
            get => RegistrySettings.AutomaticallyCheckForUpdates;
            set
            {
                var hive = Registry.CurrentUser.CreateSubKey(RegistrySettings.RTKHive);
                hive.SetValue(RegistrySettings.AutoUpdateKey, value ? 1 : 0, RegistryValueKind.DWord);
            }
        }

        [DllImport("coredll.dll", CharSet = CharSet.Unicode)]
        private static extern int SHCreateShortcut(StringBuilder szShortcut, StringBuilder szTarget);

        internal static void UpdateStartMenuShortcut(bool create)
        {
            string startMenuItem = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "Raid Toolkit.lnk");
            if (File.Exists(startMenuItem))
                File.Delete(startMenuItem);

            if (create)
                Shortcut.Create(startMenuItem, InstalledExecutablePath, "Raid Toolkit");
        }

        public static void RegisterProtocol(bool registerProtocolHandler)
        {
            try
            {
                RegistryKey classesKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes", true);
                classesKey.DeleteSubKeyTree(RegistrySettings.Protocol, false);
                if (registerProtocolHandler)
                {
                    RegistryKey classKey = classesKey.CreateSubKey(RegistrySettings.Protocol);
                    classKey.SetValue(null, "URL:Raid Toolkit");
                    classKey.SetValue("URL Protocol", "");
                    var cmdKey = classKey.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command");
                    cmdKey.SetValue(null, $"\"{InstalledExecutablePath}\" open \"%1\"");
                }
            }
            catch (Exception)
            { }
        }

        static AppConfiguration()
        {
            ExecutablePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            ExecutableName = "Raid.Service.exe";
            ExecutableDirectory = Path.GetDirectoryName(ExecutablePath);

            DefaultInstallationPath = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RaidToolkit");
        }
    }
}
