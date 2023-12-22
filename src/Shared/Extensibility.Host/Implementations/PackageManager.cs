using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Raid.Toolkit.Common;

namespace Raid.Toolkit.Extensibility.Host
{
    public class PackageManager : IPackageManager
    {
        private const string DeleteMeFile = ".delete-me";
        private const string InstallMeFile = ".install-me";
        private static string ExtensionsDirectory => Path.Combine(RegistrySettings.InstallationPath, "extensions");
        private static string DownloadsDirectory => Path.Combine(RegistrySettings.InstallationPath, "downloads");

        private readonly List<ExtensionBundle> Descriptors = new();
        private readonly ILogger<PackageManager> Logger;
        private bool IsLoaded = false;
        public static string? DebugPackage { get; set; }

        public PackageManager(ILogger<PackageManager> logger)
        {
            Logger = logger;
        }

        private bool IsPackageLoaded(string id)
        {
            return Descriptors.Any(desc => desc.Id == id);
        }

        public Task<ExtensionBundle?> RequestPackageInstall(ExtensionBundle package)
        {
            throw new V3NotImplException();
            /*
            bool result = await AppUI.ShowExtensionInstaller(package);
            if (!result)
            {
                throw new OperationCanceledException();
            }
            ExtensionBundle? installedPackage = AddPackage(package);
            return installedPackage;
            */
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
                                    ExtensionBundle.FromFile(targetPackage).Install(ExtensionsDirectory);
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogError(ex, "{targetPackage} could not be installed", targetPackage);
                                }
                                finally
                                {
                                    try
                                    {
                                        File.Delete(installMeFilePath);
                                        File.Delete(targetPackage);
                                    }
                                    catch { }
                                }
                            }
                            ExtensionBundle bundle = ExtensionBundle.FromDirectory(dir);
                            if (!string.IsNullOrEmpty(bundle.Manifest.RequireVersion))
                            {
                                if (Version.TryParse(bundle.Manifest.RequireVersion, out Version? requiredVersion) &&
                                    Version.TryParse(bundle.Manifest.RequireVersion, out Version? currentVersion))
                                {
                                    if (currentVersion < requiredVersion)
                                    {
                                        Logger.LogWarning("Extension {bundle.Id} requires version {requiredVersion} but {currentVersion} is installed", bundle.Id, requiredVersion, currentVersion);
                                        continue;
                                    }
                                }
                            }
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
            // TODO
        }

        public ExtensionBundle? AddPackage(ExtensionBundle packageToInstall)
        {
            if (IsPackageLoaded(packageToInstall.Id))
            {
                if (!Directory.Exists(DownloadsDirectory))
                    Directory.CreateDirectory(DownloadsDirectory);

                string downloadedPackageBundleTargetPath = Path.Combine(DownloadsDirectory, Path.GetFileName(packageToInstall.BundleLocation!));
                File.Copy(packageToInstall.BundleLocation!, downloadedPackageBundleTargetPath, true);

                string targetDir = packageToInstall.GetInstallDir(ExtensionsDirectory);
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);

                File.WriteAllText(Path.Combine(targetDir, InstallMeFile), downloadedPackageBundleTargetPath);

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
