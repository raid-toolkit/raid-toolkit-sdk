using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

#if NET5_0_OR_GREATER
[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif

namespace Raid.Toolkit.Common;

public enum FeatureFlags
{
    _Unused,
}
public static class RegistrySettings
{
    public const string RTKHive = @"SOFTWARE\RaidToolkit";
    public const string DebugHive = $@"{RTKHive}\Debug";
    // early access flags
    public const string FlagsHive = $@"{RTKHive}\Flags";

    public const string InstallFolderKey = @"InstallFolder";
    public const string ActivationPortKey = @"ActivationPort";
    public const string AutoUpdateKey = @"AutoUpdate";
    public const string IsInstalledKey = "IsInstalled";
    public const string FirstRunKey = "FirstRun";
    public const string ClickToStartKey = "ClickToStart";
    public const string DebugStartupKey = "DebugStartup";
    public const string RepositoryKey = "Repository";
    public const string InstallPrereleasesKey = "InstallPrereleases";
    public const string StartupName = "RaidToolkitService";
    public const string Protocol = "rtk";
    public const string StartupHive = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    public const string ExecutableName = "Raid.Toolkit.exe";
    public const string WorkerExecutableName = "Raid.Toolkit.ExtensionHost.exe";
    public const string BinDir = "bin";

    public static readonly string DefaultRepository = "raid-toolkit/raid-toolkit-sdk";
    public static readonly string DefaultInstallationPath;
    public static readonly uint DefaultActivationPort = 9998;
    public static string InstalledExecutablePath => Path.Combine(InstallationPath, BinDir, ExecutableName);
    public static string InstalledWorkerExecutablePath => Path.Combine(InstallationPath, BinDir, WorkerExecutableName);

    public static bool IsFlagEnabled(FeatureFlags flag, bool defaultValue = false)
    {
        return Registry.CurrentUser.OpenSubKey(FlagsHive)?.GetValue(flag.ToString().ToLowerInvariant(), defaultValue ? 1 : 0) is int value && value == 1;
    }

    public static uint ActivationPort
    {
        get
        {
            var hive = Registry.CurrentUser.OpenSubKey(RTKHive);
            return hive == null ? DefaultActivationPort : (uint?)hive.GetValue(ActivationPortKey, DefaultActivationPort) ?? DefaultActivationPort;
        }
        set
        {
            Registry.CurrentUser.CreateSubKey(RTKHive).SetValue(ActivationPortKey, value, RegistryValueKind.DWord);
        }
    }

    public static bool DebugStartup
    {
        get => IsSettingEnabled(DebugHive, DebugStartupKey, defaultValue: false);
        set => Registry.CurrentUser.CreateSubKey(DebugHive).SetValue(DebugStartupKey, value ? 1 : 0, RegistryValueKind.DWord);
    }

    public static bool RunOnStartup
    {
        get => DoesKeyExist(StartupHive, StartupName);
        set
        {
            if (value)
                Registry.CurrentUser.CreateSubKey(StartupHive).SetValue(StartupName, InstalledExecutablePath, RegistryValueKind.String);
            else
                Registry.CurrentUser.CreateSubKey(StartupHive).DeleteValue(StartupName, false);
        }
    }
    public static bool IsInstalled
    {
        get => IsSettingEnabled(RTKHive, IsInstalledKey);
        set => Registry.CurrentUser.CreateSubKey(RTKHive).SetValue(IsInstalledKey, 1, RegistryValueKind.DWord);
    }
    public static bool ClickToStart
    {
        get => IsSettingEnabled(RTKHive, ClickToStartKey, defaultValue: true);
        set => Registry.CurrentUser.CreateSubKey(RTKHive).SetValue(ClickToStartKey, value ? 1 : 0, RegistryValueKind.DWord);
    }
    public static bool InstallPrereleases
    {
        get => IsSettingEnabled(RTKHive, InstallPrereleasesKey);
        set => Registry.CurrentUser.CreateSubKey(RTKHive).SetValue(InstallPrereleasesKey, value ? 1 : 0, RegistryValueKind.DWord);
    }
    public static bool FirstRun
    {
        get => IsSettingEnabled(RTKHive, FirstRunKey, defaultValue: true);
        set => Registry.CurrentUser.CreateSubKey(RTKHive).SetValue(FirstRunKey, value ? 1 : 0, RegistryValueKind.DWord);
    }

    public static bool AutomaticallyCheckForUpdates
    {
        get => IsSettingEnabled(RTKHive, AutoUpdateKey);
        set
        {
            var hive = Registry.CurrentUser.CreateSubKey(RTKHive);
            hive.SetValue(AutoUpdateKey, value ? 1 : 0, RegistryValueKind.DWord);
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
            var hive = Registry.CurrentUser.CreateSubKey(RTKHive);
            hive.SetValue(InstallFolderKey, value, RegistryValueKind.String);
        }
    }

    public static string Repository
    {
        get
        {
            var hive = Registry.CurrentUser.OpenSubKey(RTKHive);
            return hive == null ? DefaultRepository : (string)hive.GetValue(RepositoryKey, DefaultRepository)!;
        }
        set
        {
            var hive = Registry.CurrentUser.CreateSubKey(RTKHive);
            hive.SetValue(RepositoryKey, value, RegistryValueKind.String);
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
