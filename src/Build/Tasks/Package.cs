using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Win32;
using Newtonsoft.Json;
using Raid.Toolkit.Common;

namespace Raid.Toolkit.Build.Tasks;

public class Package : Microsoft.Build.Utilities.Task
{
	[Required]
	public string? OutputDir { get; set; }

	[Required]
	public string? OutputFile { get; set; }

	[Required]
	public string? Install { get; set; }

	private static readonly string[] SkipOutputFiles =
		Constants.SDKAssemblies.Select(x => x + ".dll")
		.Concat(Constants.SDKAssemblies.Select(x => x + ".pdb"))
		.Concat(Constants.SDKAssemblies.Select(x => x + ".deps.json"))
		.ToArray();

	private static readonly string[] SkipOutputExtensions =
		Constants.SDKAssemblies.Select(x => x + ".pdb")
		.Concat(Constants.SDKAssemblies.Select(x => x + ".deps.json"))
		.ToArray();

	public override bool Execute()
	{
		Log.LogMessage(MessageImportance.High, $"Packaging extension {OutputFile}");

		if (string.IsNullOrEmpty(OutputFile))
		{
			Log.LogError("OutputFile must be specified");
			return false;
		}

		if (string.IsNullOrEmpty(OutputDir))
		{
			Log.LogError("OutputDir must be specified");
			return false;
		}
		OutputDir = Path.GetFullPath(OutputDir);

		string[] packagesToClean = Directory.GetFiles(OutputDir, "*.rtkx");
		foreach (string filePath in packagesToClean)
			File.Delete(filePath);

		string tempOutput = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("d"));
		using (ZipArchive archive = ZipFile.Open(tempOutput, ZipArchiveMode.Create))
		{
			string[] files = Directory.GetFiles(OutputDir, "*", SearchOption.AllDirectories);
			foreach (string filePath in files)
			{
				string relativePath = filePath.Substring(OutputDir!.Length);
				string filename = Path.GetFileName(relativePath);

				if (SkipOutputFiles.Contains(filename))
					continue;

				if (SkipOutputExtensions.Contains(Path.GetExtension(relativePath)))
					continue;

				archive.CreateEntryFromFile(filePath, relativePath);
			}
		}
		File.Move(tempOutput, OutputFile);

		if (bool.TryParse(Install, out bool value) && value)
		{
			InstallPackage();
		}

		return true;
	}

	private void InstallPackage()
	{
		var hive = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\RaidToolkit");
		{
			if (SingletonProcess.IsRunning)
			{
				Log.LogMessage(MessageImportance.High, $"Extension in use, will not deploy");
				return;
			}
			if (hive == null)
				return;

			if (hive.GetValue("InstallFolder") is not string installFolder)
				return;

			if (installFolder == null)
				return;

			string exePath = Path.Combine(installFolder, "Raid.Service.exe");
			if (!File.Exists(exePath))
				exePath = Path.Combine(installFolder, "Raid.Toolkit.exe");
			if (!File.Exists(exePath))
				return;

			Log.LogMessage(MessageImportance.High, $"Installing extension {OutputFile}");
			Process.Start(new ProcessStartInfo(exePath, $"install \"{OutputFile!}\" --accept"))?.WaitForExit();
		}
	}
}
