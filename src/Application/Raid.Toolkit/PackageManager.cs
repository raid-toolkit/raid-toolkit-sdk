using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Raid.Toolkit
{
    public class PackageManager : IPackageManager
    {
        private static string ExtensionsDirectory => Path.Combine(AppHost.ExecutableDirectory, "extensions");

        readonly List<PackageDescriptor> Descriptors = new();

        public PackageManager()
        {
            Load();
        }

        private void Load()
        {
            if (Descriptors.Count > 0) return;

            // add packaged extensions
            Descriptors.Add(DescriptorFor<Extension.Account.AccountExtension>());
            Descriptors.Add(DescriptorFor<Extension.Realtime.RealtimeExtension>());

            if (Directory.Exists(ExtensionsDirectory))
            {
                string[] files = Directory.GetFiles(ExtensionsDirectory, "Raid.Toolkit.Extension.*.dll");
                foreach (string file in files)
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(file);
                    Descriptors.Add(new(Path.GetFileNameWithoutExtension(file), fvi.ProductName, fvi.FileDescription, file));
                }
            }
        }

        private static PackageDescriptor DescriptorFor<T>()
        {
            return PackageDescriptor.FromAssembly(typeof(T).Assembly);
        }

        public PackageDescriptor AddPackage(PackageDescriptor packageToInstall)
        {
            Directory.CreateDirectory(ExtensionsDirectory);
            string newLocation = Path.Combine(ExtensionsDirectory, Path.GetFileName(packageToInstall.Location));
            if (File.Exists(newLocation))
            {
                throw new InvalidOperationException("An extension with that name already exists");
            }
            File.Copy(packageToInstall.Location, newLocation);
            PackageDescriptor descriptor = new(packageToInstall.Id, packageToInstall.Name, packageToInstall.Description, newLocation);
            Descriptors.Add(descriptor);
            return descriptor;
        }

        public IEnumerable<PackageDescriptor> GetAllPackages()
        {
            return Descriptors;
        }

        public PackageDescriptor GetPackage(string packageId)
        {
            return Descriptors.Single(d => d.Id == packageId);
        }
    }
}
