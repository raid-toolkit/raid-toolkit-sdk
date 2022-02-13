using Raid.Toolkit.Extensibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Raid.Toolkit
{
    public class PackageLocator : IPackageLocator
    {
        readonly List<PackageDescriptor> Descriptors = new();

        public PackageLocator()
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
