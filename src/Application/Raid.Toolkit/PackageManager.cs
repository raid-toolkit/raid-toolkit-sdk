using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.Injection;

namespace Raid.Toolkit
{
    public class PackageManager : IPackageManager
    {
        private static string ExtensionsDirectory => Path.Combine(RegistrySettings.InstallationPath, "extensions");

        readonly List<ExtensionBundle> Descriptors = new();
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
            // let's get fucky
            ClientApi.Preload();
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
                    // load legacy extensions:
                    string[] files = Directory.GetFiles(ExtensionsDirectory, "Raid.Toolkit.Extension.*.dll");
                    var legacyBundles = files.Select(file => ExtensionBundle.FromAssembly(file));
                    foreach (var legacyBundle in legacyBundles)
                        descriptors[legacyBundle.Id] = legacyBundle;

                    string[] dirs = Directory.GetDirectories(ExtensionsDirectory);
                    foreach (string dir in dirs)
                    {
                        try
                        {
                            ExtensionBundle bundle = ExtensionBundle.FromDirectory(dir);
                            descriptors[bundle.Id] = bundle; // overwrite any legacy extensions
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
    }
}
