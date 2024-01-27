using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Loader;

namespace Raid.Toolkit.Build.Tasks;

public class Codegen : Microsoft.Build.Utilities.Task
{
	[Required]
	public string[]? ManifestFiles { get; set; }

	[Required]
	public string? OutputFile { get; set; }

	[Required]
	public string? OutDir { get; set; }

	public string? CacheDir { get; set; }

	public override bool Execute()
	{
		Log.LogMessage($"Writing to {OutputFile}...");

		if (string.IsNullOrEmpty(OutputFile))
		{
			Log.LogError("OutputFile must be specified");
			return false;
		}
		if (string.IsNullOrEmpty(OutDir))
		{
			Log.LogError("OutDir must be specified");
			return false;
		}
		if (ManifestFiles == null || ManifestFiles.Length == 0)
		{
			Log.LogError("Build requires a manifest. Verify your manifest file [Build Action] is set to 'GenerateRTKManifest'.");
			return false;
		}
		if (ManifestFiles.Length > 1)
		{
			Log.LogError("Build requires a manifest. Verify your manifest file [Build Action] is set to 'GenerateRTKManifest'.");
			return false;
		}

		PackageManifest manifest = JsonConvert.DeserializeObject<PackageManifest>(File.ReadAllText(ManifestFiles[0]))!;

		string cacheDir = Path.Combine(Path.GetTempPath(), "_rtkBuildCache", manifest.Id);
		_ = Directory.CreateDirectory(cacheDir);

		ModelLoader loader = new(null, Options.Create(new ModelLoaderOptions()));
		string tempOutputFileCache = Path.Combine(cacheDir, loader.OutputFilename);

		try
		{
			List<Regex> patternMatchers = manifest.Codegen?.Types?
				.Select(pattern => new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled)).ToList()
				?? new();
			string dllPath = loader.Build(patternMatchers, cacheDir).GetResultSync();
			if (File.Exists(OutputFile))
				File.Delete(OutputFile);

			File.Copy(dllPath, OutputFile);
			if (!string.IsNullOrEmpty(CacheDir))
			{
				try
				{
					string cachedOutputFile = Path.Combine(CacheDir, loader.OutputFilename);
					Log.LogMessage(MessageImportance.High, $"Caching artifact at: {cachedOutputFile}");
					_ = Directory.CreateDirectory(CacheDir);
					if (File.Exists(cachedOutputFile))
						File.Delete(cachedOutputFile);
					File.Copy(dllPath, cachedOutputFile);
				}
				catch { }
			}
		}
		catch (Exception ex)
		{
			if (!string.IsNullOrEmpty(CacheDir))
			{
				string cachedOutputFile = Path.Combine(CacheDir, loader.OutputFilename);
				Log.LogMessage(MessageImportance.High, "Failed to generate, attempting to load from cache");
				if (!File.Exists(cachedOutputFile))
				{
					Log.LogError(ex.ToString());
					throw new FileNotFoundException("Could not generate interop dll and none was present in cache", tempOutputFileCache);
				}

				if (File.Exists(OutputFile))
					File.Delete(OutputFile);
				Log.LogMessage(MessageImportance.High, $"Using cached artifact: {cachedOutputFile}");
				File.Copy(cachedOutputFile, OutputFile);
			}
			else
			{
				Log.LogError(ex.ToString());
				throw new FileNotFoundException("Could not generate interop dll and none was present in cache", tempOutputFileCache);
			}
		}

		_ = Directory.CreateDirectory(OutDir);
		string outputManifest = Path.Combine(OutDir, Constants.ExtensionManifestFileName);
		if (File.Exists(outputManifest))
			File.Delete(outputManifest);
		manifest.RequireVersion ??= "3.0.0";
		manifest.CompatibleVersion ??= Version.Parse(ThisAssembly.AssemblyVersion);
		File.WriteAllText(outputManifest, JsonConvert.SerializeObject(manifest));

		return true;
	}
}
