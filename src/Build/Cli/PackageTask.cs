using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.Versioning;
using CommandLine;
using Raid.Toolkit.Common;

namespace Raid.Toolkit.Build.Cli;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


[Verb("package", HelpText = "Packages an extension")]
public class PackageTaskArgs : ITaskArgs
{
	[Option('o', "out-dir", HelpText = "Project output directory to package", Required = true)]
	public string OutputDir { get; set; }

	[Option('p', "out-file", HelpText = "Output file", Required = true)]
	public string OutputFile { get; set; }

	[Option('i', "install", HelpText = "Install the package after building", Default = false)]
	public bool Install { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class PackageTask
{
	public static int Execute(PackageTaskArgs args)
	{
		if (!OperatingSystem.IsWindows())
		{
			Console.Error.WriteLine("This tool is only supported on Windows");
			return -1;
		}
		Console.WriteLine($"Packaging extension {args.OutputFile}");

		string[] packagesToClean = Directory.GetFiles(args.OutputFile, "*.rtkx");
		foreach (string filePath in packagesToClean)
			File.Delete(filePath);

		string tempOutput = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("d"));
		ZipFile.CreateFromDirectory(args.OutputFile, tempOutput);
		File.Move(tempOutput, args.OutputFile);

		int result = 0;
		if (args.Install)
		{
			result = InstallPackage(args);
		}

		return result;
	}

	[SupportedOSPlatform("windows")]
	private static int InstallPackage(PackageTaskArgs args)
	{
		string? installFolder = RegistrySettings.InstallationPath;
		if (installFolder == null)
			return 0;

		string exePath = Path.Combine(installFolder, RegistrySettings.WorkerExecutableName);
		if (!File.Exists(exePath))
		{
			Console.Error.WriteLine($"Cannot install extension, {exePath} does not exist");
			return 1;
		}

		Console.WriteLine($"Installing extension {args.OutputFile}");
		Process.Start(new ProcessStartInfo(exePath, $"install \"{args.OutputFile!}\" --accept"))?.WaitForExit();
		return 0;
	}
}
