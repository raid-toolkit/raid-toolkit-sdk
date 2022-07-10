using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

#if NET5_0_OR_GREATER
[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif

namespace Raid.Toolkit.Common
{
    public enum FeatureFlags
    {
        _Unused,
    }
    public static class RegistrySettings
    {
        public const string RTKHive = @"SOFTWARE\RaidToolkit";
        public const string InstallFolderKey = @"InstallFolder";
        public const string AutoUpdateKey = @"AutoUpdate";
        public const string IsInstalledKey = "IsInstalled";
        public const string ClickToStartKey = "ClickToStart";
        public const string InstallPrereleasesKey = "InstallPrereleases";
        public const string StartupName = "RaidToolkitService";
        public const string Protocol = "rtk";
        public const string StartupHive = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        public const string ExecutableName = "Raid.Toolkit.exe";
        // early access flags
        public const string FlagsHive = $"{RTKHive}\\Flags";

        public static readonly string DefaultInstallationPath;
        public static string InstalledExecutablePath => Path.Combine(InstallationPath, ExecutableName);

        public static bool IsFlagEnabled(FeatureFlags flag, bool defaultValue = false)
        {
            return Registry.CurrentUser.OpenSubKey(FlagsHive)?.GetValue(flag.ToString().ToLowerInvariant(), defaultValue ? 1 : 0) is int value && value == 1;
        }

        public static bool RunOnStartup
        {
            get => DoesKeyExist(StartupHive, StartupName);
            set
            {
                if (value)
                    Registry.CurrentUser.CreateSubKey(RegistrySettings.StartupHive).SetValue(RegistrySettings.StartupName, InstalledExecutablePath, RegistryValueKind.String);
                else
                    Registry.CurrentUser.CreateSubKey(RegistrySettings.StartupHive).DeleteValue(RegistrySettings.StartupName, false);
            }
        }
        public static bool IsInstalled
        {
            get => IsSettingEnabled(RTKHive, IsInstalledKey);
            set => Registry.CurrentUser.CreateSubKey(RegistrySettings.RTKHive).SetValue(RegistrySettings.IsInstalledKey, 1, RegistryValueKind.DWord);
        }
        public static bool ClickToStart
        {
            get => IsSettingEnabled(RTKHive, ClickToStartKey, defaultValue: true);
            set => Registry.CurrentUser.CreateSubKey(RegistrySettings.RTKHive).SetValue(RegistrySettings.ClickToStartKey, value ? 1 : 0, RegistryValueKind.DWord);
        }
        public static bool InstallPrereleases
        {
            get => IsSettingEnabled(RTKHive, InstallPrereleasesKey);
            set => Registry.CurrentUser.CreateSubKey(RegistrySettings.RTKHive).SetValue(RegistrySettings.InstallPrereleasesKey, value ? 1 : 0, RegistryValueKind.DWord);
        }

        public static bool AutomaticallyCheckForUpdates
        {
            get => IsSettingEnabled(RTKHive, AutoUpdateKey);
            set
            {
                var hive = Registry.CurrentUser.CreateSubKey(RegistrySettings.RTKHive);
                hive.SetValue(RegistrySettings.AutoUpdateKey, value ? 1 : 0, RegistryValueKind.DWord);
            }
        }

        public static string InstallationPath
        {
            get
            {
                var hive = Registry.CurrentUser.OpenSubKey(RTKHive);
                return hive == null ? DefaultInstallationPath : (string)hive.GetValue(InstallFolderKey, DefaultInstallationPath)!;
            }
            set
            {
                var hive = Registry.CurrentUser.CreateSubKey(RegistrySettings.RTKHive);
                hive.SetValue(RegistrySettings.InstallFolderKey, value, RegistryValueKind.String);
            }
        }

        [DllImport("coredll.dll", CharSet = CharSet.Unicode)]
        private static extern int SHCreateShortcut(StringBuilder szShortcut, StringBuilder szTarget);

        public static void UpdateStartMenuShortcut(bool create)
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
                classesKey?.DeleteSubKeyTree(Protocol, false);

                if (registerProtocolHandler && classesKey != null)
                {
                    RegistryKey classKey = classesKey.CreateSubKey(Protocol);
                    classKey.SetValue(null, "URL:Raid Toolkit");
                    classKey.SetValue("URL Protocol", "");
                    var cmdKey = classKey.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command");
                    cmdKey.SetValue(null, $"\"{InstalledExecutablePath}\" open \"%1\"");
                }
            }
            catch (Exception)
            { }
        }

        private static bool IsSettingEnabled(string path, string key, bool defaultValue = false)
        {
            var value = Registry.CurrentUser.OpenSubKey(path)?.GetValue(key, defaultValue ? 1 : 0);
            return value is not null and int intValue && intValue != 0;
        }

        private static bool DoesKeyExist(string path, string key)
        {
            return Registry.CurrentUser.OpenSubKey(path)?.GetValue(key) != null;
        }

        static RegistrySettings()
        {
            DefaultInstallationPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RaidToolkit");
        }
    }
}
