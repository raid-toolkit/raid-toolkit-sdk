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
        public const string StartupName = "RaidToolkitService";
        public const string Protocol = "rtk";
        public const string StartupHive = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        public const string ExecutableName = "Raid.Service.exe";

        public static readonly string DefaultInstallationPath;

        public static string InstalledExecutablePath => Path.Join(InstallationPath, ExecutableName);
        public static bool RunOnStartup => Registry.CurrentUser.OpenSubKey(StartupHive).GetValue(StartupName) != null;
        public static bool IsInstalled => Registry.CurrentUser.OpenSubKey(RTKHive)?.GetValue(IsInstalledKey) != null;

        public static string InstallationPath
        {
            get
            {
                var hive = Registry.CurrentUser.OpenSubKey(RTKHive);
                if (hive == null)
                    return DefaultInstallationPath;

                return (string)hive.GetValue(InstallFolderKey, DefaultInstallationPath);
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
        }

        static RegistrySettings()
        {
            DefaultInstallationPath = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RaidToolkit");
        }
    }
}