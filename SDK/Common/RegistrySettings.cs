using System;
using System.IO;
using Microsoft.Win32;

[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]

namespace Raid.Common
{
    public static class RegistrySettings
    {
        public const string RTKHive = @"SOFTWARE\RaidToolkit";
        public const string InstallFolderKey = @"InstallFolder";
        public const string AutoUpdateKey = @"AutoUpdate";
        public const string IsInstalledKey = "IsInstalled";
        public const string ClickToStartKey = "ClickToStart";
        public const string StartupName = "RaidToolkitService";
        public const string Protocol = "rtk";
        public const string StartupHive = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        public const string ExecutableName = "Raid.Service.exe";

        public static readonly string DefaultInstallationPath;

        public static string InstalledExecutablePath => Path.Join(InstallationPath, ExecutableName);
        public static bool RunOnStartup => IsSettingEnabled(StartupHive, StartupName);
        public static bool IsInstalled => IsSettingEnabled(RTKHive, IsInstalledKey);
        public static bool ClickToStart => IsSettingEnabled(RTKHive, ClickToStartKey);
        public static bool AutomaticallyCheckForUpdates => IsSettingEnabled(RTKHive, AutoUpdateKey);

        public static string InstallationPath
        {
            get
            {
                var hive = Registry.CurrentUser.OpenSubKey(RTKHive);
                return hive == null ? DefaultInstallationPath : (string)hive.GetValue(InstallFolderKey, DefaultInstallationPath);
            }
        }

        private static bool IsSettingEnabled(string path, string key)
        {
            var value = Registry.CurrentUser.OpenSubKey(path)?.GetValue(key, 0);
            return value is not null and int intValue && intValue != 0;
        }

        static RegistrySettings()
        {
            DefaultInstallationPath = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RaidToolkit");
        }
    }
}