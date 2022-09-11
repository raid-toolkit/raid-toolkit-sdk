using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host;

namespace Raid.Toolkit.DependencyInjection
{
    public class PackageManager : IPackageManager
    {
        private const string DeleteMeFile = ".delete-me";
        private static string ExtensionsDirectory => Path.Combine(RegistrySettings.InstallationPath, "extensions");

        private readonly List<ExtensionBundle> Descriptors = new();
        private bool IsLoaded = false;
        public static string? DebugPackage { get; set; }
        public static bool NoDefaultPackages = false;

        public PackageManager()
        {
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
                                catch(Exception)
                                {
                                    try
                                    {
                                        File.WriteAllText(Path.Combine(dir, DeleteMeFile), "");
                                    }
                                    catch (Exception) { }
                                }
                                continue;
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

        public ExtensionBundle AddPackage(ExtensionBundle packageToInstall)
        {
            Directory.CreateDirectory(ExtensionsDirectory);
            packageToInstall.Install(ExtensionsDirectory);
            return ExtensionBundle.FromDirectory(packageToInstall.GetInstallDir(ExtensionsDirectory));
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
            if  (Directory.Exists(packageDir))
            {
                try
                {
                    Directory.Delete(packageDir, true);
                }
                catch (Exception)
                {
                    File.WriteAllText(Path.Combine(packageDir, DeleteMeFile), "");
                }
            }
        }
    }
}
