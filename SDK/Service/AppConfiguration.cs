using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

namespace Raid.Service
{
    public static class AppConfiguration
    {
        private const string RTKHive = @"SOFTWARE\RaidToolkit";
        private const string InstallFolderKey = @"InstallFolder";
        private const string AutoUpdateKey = @"AutoUpdate";
        private const string IsInstalledKey = "IsInstalled";
        private const string StartupName = "RaidToolkitService";
        private const string Protocol = "rtk";
        private const string StartupHive = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

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

        public static bool IsInstalled
        {
            get => Registry.CurrentUser.OpenSubKey(RTKHive)?.GetValue(IsInstalledKey) != null;
            set => Registry.CurrentUser.CreateSubKey(RTKHive).SetValue(IsInstalledKey, 1, RegistryValueKind.DWord);
        }

        public static string InstalledExecutablePath => Path.Join(InstallationPath, ExecutableName);

        public static string InstallationPath
        {
            get
            {
                var hive = Registry.CurrentUser.OpenSubKey(RTKHive);
                if (hive == null)
                    return DefaultInstallationPath;

                return (string)hive.GetValue(InstallFolderKey, DefaultInstallationPath);
            }
            set
            {
                var hive = Registry.CurrentUser.CreateSubKey(RTKHive);
                hive.SetValue(InstallFolderKey, value, RegistryValueKind.String);
            }
        }

        public static bool RunOnStartup
        {
            get => Registry.CurrentUser.OpenSubKey(StartupHive).GetValue(StartupName) != null;
            set
            {
                if (value)
                    Registry.CurrentUser.CreateSubKey(StartupHive).SetValue(StartupName, InstalledExecutablePath, RegistryValueKind.String);
                else
                    Registry.CurrentUser.CreateSubKey(StartupHive).DeleteValue(StartupName);
            }
        }

        public static bool AutomaticallyCheckForUpdates
        {
            get
            {
                var hive = Registry.CurrentUser.OpenSubKey(RTKHive);
                if (hive == null)
                    return false;

                return (int)hive.GetValue(AutoUpdateKey, DefaultInstallationPath) != 0;
            }
            set
            {
                var hive = Registry.CurrentUser.CreateSubKey(RTKHive);
                hive.SetValue(AutoUpdateKey, value ? 1 : 0, RegistryValueKind.DWord);
            }
        }

        [DllImport("coredll.dll")]
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
            RegistryKey classesKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes", true);
            classesKey.DeleteSubKeyTree(Protocol, false);
            if (registerProtocolHandler)
            {
                RegistryKey classKey = classesKey.CreateSubKey(Protocol);
                classKey.SetValue(null, "URL:Raid Toolkit");
                classKey.SetValue("URL Protocol", "");
                var cmdKey = classKey.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command");
                cmdKey.SetValue(null, $"\"{InstalledExecutablePath}\" open \"%1\"");
            }
        }

        static AppConfiguration()
        {
            ExecutablePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            ExecutableName = Path.GetFileName(ExecutablePath);
            ExecutableDirectory = Path.GetDirectoryName(ExecutablePath);

            DefaultInstallationPath = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RaidToolkit");
        }
    }
}