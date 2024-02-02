using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using Newtonsoft.Json;

namespace Raid.Toolkit.Extensibility;

public sealed class ExtensionBundle
{
	public Assembly? Assembly { get; private set; }
	public PackageManifest Manifest { get; }
	public string? Location { get; private set; }
	public string? BundleLocation { get; private set; }
	public string Id => Manifest.Id;
	public bool IsLinkedAssembly { get; private set; }

	public ExtensionBundle(PackageManifest manifest)
	{
		Manifest = manifest;
	}

	public string GetExtensionEntrypointDll()
	{
		if (!string.IsNullOrEmpty(BundleLocation))
			throw new ApplicationException("Cannot load this extension. It is not installed");
		if (string.IsNullOrEmpty(Location))
			throw new ApplicationException("Cannot load an extension without a location");
		return Path.Combine(Location, Manifest.Assembly);
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

		if (!string.IsNullOrEmpty(BundleLocation))
		{
			ZipFile.ExtractToDirectory(BundleLocation, installationPath);
		}
	}

	public static ExtensionBundle FromType<T>()
	{
		return FromAssembly(typeof(T).Assembly);
	}

	public void WriteManifest(PackageManifest manifest)
	{
		JsonSerializer serializer = new();
		using StreamWriter writer = new(Path.Join(Location!, Constants.ExtensionManifestFileName));
		using JsonTextWriter textWriter = new(writer);
		serializer.Serialize(textWriter, manifest);
	}

	public static ExtensionBundle FromAssembly(Assembly assembly)
	{
		PackageManifest manifest = PackageManifest.FromAssembly(assembly);
		return new(manifest)
		{
			Assembly = assembly,
			IsLinkedAssembly = true
		};
	}

	public static ExtensionBundle FromFile(string filename)
	{
		using ZipArchive arch = ZipFile.OpenRead(filename);
		ZipArchiveEntry? manifestEntry = arch.GetEntry(Constants.ExtensionManifestFileName)
			?? throw new ApplicationException($"Extension package '{filename}' does not contain a valid manifest");
		using Stream manifestStream = manifestEntry.Open();
		PackageManifest manifest = ReadManifest(manifestStream);
		return new(manifest)
		{
			Location = Path.GetDirectoryName(filename),
			BundleLocation = filename
		};
	}

	public static ExtensionBundle FromDirectory(string dirname)
	{
		string manifestFile = Path.Combine(dirname, Constants.ExtensionManifestFileName);
		if (!File.Exists(manifestFile))
		{
			throw new ApplicationException($"Extension package '{dirname}' does not contain a valid manifest");
		}
		using Stream manifestStream = File.OpenRead(manifestFile);
		PackageManifest manifest = ReadManifest(manifestStream);
		return new(manifest)
		{
			Location = dirname
		};
	}

	private static PackageManifest ReadManifest(Stream manifestStream)
	{
		JsonSerializer serializer = new();
		using StreamReader reader = new(manifestStream);
		using JsonTextReader textReader = new(reader);
		PackageManifest? manifest = serializer.Deserialize<PackageManifest>(textReader)
			?? throw new ApplicationException("Could not deserialize ExtensionManifest");
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
