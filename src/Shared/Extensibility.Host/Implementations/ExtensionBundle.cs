using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using Newtonsoft.Json;

namespace Raid.Toolkit.Extensibility
{
    public sealed class ExtensionBundle
    {
        public Assembly Assembly { get; private set; }
        public ExtensionManifest Manifest { get; }
        public string Location { get; private set; }
        public string Id => Manifest.Id;

        private string ZipPath;

        public ExtensionBundle(ExtensionManifest manifest)
        {
            Manifest = manifest;
        }

        public string GetExtensionEntrypointDll()
        {
            return string.IsNullOrEmpty(Location)
                ? throw new ApplicationException("Cannot load this extension, as it is not installed")
                : Path.Combine(Location, Manifest.Assembly);
        }

        public string GetInstallDir(string rootDir)
        {
            return Path.Combine(rootDir, Manifest.Id);
        }

        public void Install(string extensionsDir)
        {
            string installationPath = GetInstallDir(extensionsDir);
            if (Directory.Exists(installationPath))
            {
                DirectoryInfo di = new(installationPath);
                di.Delete(recursive: true);
            }
            _ = Directory.CreateDirectory(installationPath);

            if (!string.IsNullOrEmpty(Location))
            {
                CopyDirectory(Location, installationPath, true);
            }
            else if (!string.IsNullOrEmpty(ZipPath))
            {
                ZipFile.ExtractToDirectory(ZipPath, installationPath);
            }
        }

        public static ExtensionBundle FromType<T>()
        {
            return FromAssembly(typeof(T).Assembly);
        }

        public static ExtensionBundle FromAssembly(string assemblyPath)
        {
            return FromAssembly(Assembly.LoadFrom(assemblyPath));
        }

        public static ExtensionBundle FromAssembly(Assembly assembly)
        {
            ExtensionManifest manifest = ExtensionManifest.FromAssembly(assembly);
            return new(manifest)
            {
                Assembly = assembly
            };
        }

        public static ExtensionBundle FromFile(string filename)
        {
            using ZipArchive arch = ZipFile.OpenRead(filename);
            ZipArchiveEntry manifestEntry = arch.GetEntry(".rtk.extension.json");
            if (manifestEntry == null)
            {
                throw new ApplicationException($"Extension package '{filename}' does not contain a valid manifest");
            }
            using Stream manifestStream = manifestEntry.Open();
            ExtensionManifest manifest = ReadManifest(manifestStream);
            return new(manifest)
            {
                ZipPath = filename
            };
        }

        public static ExtensionBundle FromDirectory(string dirname)
        {
            string manifestFile = Path.Combine(dirname, ".rtk.extension.json");
            if (!File.Exists(manifestFile))
            {
                throw new ApplicationException($"Extension package '{dirname}' does not contain a valid manifest");
            }
            using Stream manifestStream = File.OpenRead(manifestFile);
            ExtensionManifest manifest = ReadManifest(manifestStream);
            return new(manifest)
            {
                Location = dirname
            };
        }

        private static ExtensionManifest ReadManifest(Stream manifestStream)
        {
            JsonSerializer serializer = new();
            using StreamReader reader = new(manifestStream);
            using JsonTextReader textReader = new(reader);
            ExtensionManifest manifest = serializer.Deserialize<ExtensionManifest>(textReader);
            return manifest;
        }

        private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            _ = Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                _ = file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
    }
}
