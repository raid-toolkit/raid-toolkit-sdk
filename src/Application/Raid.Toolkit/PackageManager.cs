using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Raid.Toolkit
{
    public class PackageManager : IPackageManager
    {
        readonly List<PackageDescriptor> Descriptors = new();

        public PackageManager()
        {
            Load();
        }

        private void Load()
        {
            if (Descriptors.Count > 0) return;
            string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Raid.Toolkit.Extension.*.dll");
            foreach (string file in files)
            {
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(file);
                Descriptors.Add(new(Path.GetFileNameWithoutExtension(file), fvi.ProductName, fvi.FileDescription, file));
            }
        }

        public PackageDescriptor AddPackage(PackageDescriptor packageToInstall)
        {
            string newLocation = Path.Join(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(packageToInstall.Location));
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
