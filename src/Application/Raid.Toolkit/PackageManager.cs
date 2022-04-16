using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host;

namespace Raid.Toolkit
{
    public class PackageManager : IPackageManager
    {
        private static string ExtensionsDirectory => Path.Combine(AppHost.ExecutableDirectory, "extensions");

        readonly List<ExtensionBundle> Descriptors = new();
        public static string? DebugPackage { get; set; }

        public PackageManager()
        {
            Load();
        }

        private void Load()
        {
            if (Descriptors.Count > 0) return;

            // add packaged extensions
            Descriptors.Add(ExtensionBundle.FromType<Extension.Account.AccountExtension>());
            Descriptors.Add(ExtensionBundle.FromType<Extension.Realtime.RealtimeExtension>());

            Dictionary<string, ExtensionBundle> descriptors = new();
            if (Directory.Exists(ExtensionsDirectory))
            {
                // load legacy extensions:
                string[] files = Directory.GetFiles(ExtensionsDirectory, "Raid.Toolkit.Extension.*.dll");
                var legacyBundles = files.Select(file => ExtensionBundle.FromFile(file));
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
            return Descriptors;
        }

        public ExtensionBundle GetPackage(string packageId)
        {
            return Descriptors.Single(d => d.Manifest.Id == packageId);
        }
    }
}
