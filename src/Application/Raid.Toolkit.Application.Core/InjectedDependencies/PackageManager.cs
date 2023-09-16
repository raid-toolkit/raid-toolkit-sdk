using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;

using CommunityToolkit.WinUI.Notifications;

using Microsoft.Extensions.Logging;

using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.Extensibility.Interfaces;
using Raid.Toolkit.Extensibility.Notifications;

namespace Raid.Toolkit.Application.Core.DependencyInjection
{
    public class PackageManager : IPackageManager
    {
        private const string DeleteMeFile = ".delete-me";
        private const string InstallMeFile = ".install-me";
        private static string ExtensionsDirectory => Path.Combine(RegistrySettings.InstallationPath, "extensions");

        private readonly List<ExtensionBundle> Descriptors = new();
        private readonly IAppUI AppUI;
        private readonly INotificationManager NotificationManager;
        private readonly IAppService AppService;
        private readonly NotificationSink PackageUpdateNotify;
        private readonly ILogger<PackageManager> Logger;
        private bool IsLoaded = false;
        public static string? DebugPackage { get; set; }
        public static bool NoDefaultPackages = false;

        public PackageManager(ILogger<PackageManager> logger, IAppUI appUI, INotificationManager notificationManager, IAppService appService)
        {
            Logger = logger;
            AppUI = appUI;
            NotificationManager = notificationManager;
            AppService = appService;
            PackageUpdateNotify = new("packageManager");
            PackageUpdateNotify.Activated += Sink_Activated;
            NotificationManager.RegisterHandler(PackageUpdateNotify);
        }

        private bool IsPackageLoaded(string id)
        {
            return Descriptors.Any(desc => desc.Id == id);
        }

        private void Sink_Activated(object? sender, NotificationActivationEventArgs e)
        {
            if (e.Arguments.TryGetValue(NotificationConstants.Action, out string? action))
            {
                switch (action)
                {
                    case "restart":
                        {
                            AppService.Restart(false);
                            break;
                        }
                }
            }
        }

        public async Task<ExtensionBundle?> RequestPackageInstall(ExtensionBundle package)
        {
            bool result = await AppUI.ShowExtensionInstaller(package);
            if (!result)
            {
                throw new OperationCanceledException();
            }
            ExtensionBundle? installedPackage = AddPackage(package);
            return installedPackage;
        }

        private void EnsureLoaded()
        {
            if (IsLoaded)
                return;

            // preload injection client type/asm before extensions get loaded
            typeof(Il2CppToolkit.Injection.Client.InjectionClient).FullName?.ToString();
            // let's get fucky.
            Load();
            IsLoaded = true;
        }

        private void Load()
        {
            if (Descriptors.Count > 0) return;

            // add packaged extensions
            if (!NoDefaultPackages)
            {
                Descriptors.Add(ExtensionBundle.FromType<Extension.Account.AccountExtension>());
                Descriptors.Add(ExtensionBundle.FromType<Extension.Realtime.RealtimeExtension>());
            }

            Dictionary<string, ExtensionBundle> descriptors = new();
            if (string.IsNullOrEmpty(DebugPackage))
            {
                if (Directory.Exists(ExtensionsDirectory))
                {
                    string[] dirs = Directory.GetDirectories(ExtensionsDirectory);
                    foreach (string dir in dirs)
                    {
                        try
                        {
                            if (File.Exists(Path.Combine(dir, DeleteMeFile)))
                            {
                                try
                                {
                                    Directory.Delete(dir, true);
                                }
                                catch (Exception)
                                {
                                    try
                                    {
                                        File.WriteAllText(Path.Combine(dir, DeleteMeFile), "");
                                    }
                                    catch (Exception) { }
                                }
                                continue;
                            }
                            else if (File.Exists(Path.Combine(dir, InstallMeFile)))
                            {
                                string installMeFilePath = Path.Combine(dir, InstallMeFile);
                                string targetPackage = File.ReadAllText(installMeFilePath);
                                if (string.IsNullOrEmpty(targetPackage))
                                {
                                    Logger.LogWarning("{installMeFilePath} does not contain a extension path", installMeFilePath);
                                    File.Delete(installMeFilePath);
                                }
                                else if (!Path.GetDirectoryName(installMeFilePath)!.Equals(dir, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    Logger.LogError("{installMeFilePath} refers to a file outside of the extension directory! '{targetPackage}'", installMeFilePath, targetPackage);
                                    File.Delete(installMeFilePath);
                                }
                                else if (!File.Exists(targetPackage))
                                {
                                    Logger.LogError("{targetPackage} does not exist!", targetPackage);
                                    File.Delete(installMeFilePath);
                                }
                                try
                                {
                                    Directory.Delete(dir, true);
                                    ExtensionBundle.FromFile(targetPackage).Install(ExtensionsDirectory);
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogError(ex, "{targetPackage} could not be installed", targetPackage);
                                }
                                finally
                                {
                                    File.Delete(installMeFilePath);
                                    File.Delete(targetPackage);
                                }
                            }
                            ExtensionBundle bundle = ExtensionBundle.FromDirectory(dir);
                            descriptors[bundle.Id] = bundle;
                        }
                        catch (Exception)
                        { }
                    }
                }
            }

            if (!string.IsNullOrEmpty(DebugPackage))
            {
                var debugPkg = ExtensionBundle.FromDirectory(DebugPackage);
                descriptors.Add(debugPkg.Id, debugPkg);
            }

            Descriptors.AddRange(descriptors.Values);
        }

        private void RequestRestart()
        {
            ToastContentBuilder tcb = new ToastContentBuilder()
                .AddText("Restart required")
                .AddText($"Raid Toolkit needs to be restarted to apply extension changes.")
                .AddButton(new ToastButton("Restart", PackageUpdateNotify.GetArguments("restart")))
                .AddButton(new ToastButtonDismiss());
            PackageUpdateNotify.SendNotification(tcb.Content, "extensions-updated");
        }

        public ExtensionBundle? AddPackage(ExtensionBundle packageToInstall)
        {
            if (IsPackageLoaded(packageToInstall.Id))
            {
                string targetDir = packageToInstall.GetInstallDir(ExtensionsDirectory);
                string targetFile = Path.Combine(targetDir, Path.GetFileName(packageToInstall.BundleLocation));
                File.Copy(packageToInstall.BundleLocation, targetFile, true);
                File.WriteAllText(Path.Combine(targetDir, InstallMeFile), targetFile);
                RequestRestart();
                return null;
            }
            else
            {
                Directory.CreateDirectory(ExtensionsDirectory);
                packageToInstall.Install(ExtensionsDirectory);
                RequestRestart();
                return ExtensionBundle.FromDirectory(packageToInstall.GetInstallDir(ExtensionsDirectory));
            }
        }

        public IEnumerable<ExtensionBundle> GetAllPackages()
        {
            EnsureLoaded();
            return Descriptors;
        }

        public ExtensionBundle GetPackage(string packageId)
        {
            EnsureLoaded();
            return Descriptors.Single(d => d.Manifest.Id == packageId);
        }

        public void RemovePackage(string packageId)
        {
            string packageDir = Path.Combine(ExtensionsDirectory, packageId);
            if (Directory.Exists(packageDir))
            {
                try
                {
                    Directory.Delete(packageDir, true);
                }
                catch (Exception)
                {
                    File.WriteAllText(Path.Combine(packageDir, DeleteMeFile), "");
                }
                RequestRestart();
            }
        }
    }
}
