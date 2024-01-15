using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using CommandLine;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Loader;

namespace Raid.Toolkit.Build.Cli;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

[Verb("codegen", HelpText = "Generates an extension interop assembly")]
public class CodegenTaskArgs : ITaskArgs
{
	[Value(0, HelpText = "Manifest file", Required = true)]
	public string ManifestFile { get; set; }

	[Option('o', "out-dir", HelpText = "Project output directory to package", Required = true)]
	public string OutputDir { get; set; }

	[Option('o', "out-asm", HelpText = "Output assembly", Required = true)]
	public string OutputFile { get; set; }

	[Option('c', "cache-dir", HelpText = "Directory to cache generated assemblies")]
	public string? CacheDir { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class CodegenTask
{
	public static int Execute(CodegenTaskArgs args)
	{
		if (!OperatingSystem.IsWindows())
		{
			Console.Error.WriteLine("This tool is only supported on Windows");
			return -1;
		}
		PackageManifest manifest = JsonConvert.DeserializeObject<PackageManifest>(File.ReadAllText(args.ManifestFile))!;

		if (manifest.Codegen?.Types == null)
		{
			Console.WriteLine("Warning: No value supplied for Codegen.Types in extension manifest, interop generation will not be performed.");
			return 0;
		}

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
			if (File.Exists(args.OutputFile))
				File.Delete(args.OutputFile);

			File.Copy(dllPath, args.OutputFile);
			if (!string.IsNullOrEmpty(args.CacheDir))
			{
				try
				{
					string cachedOutputFile = Path.Combine(args.CacheDir, loader.OutputFilename);
					Console.WriteLine($"Caching artifact at: {cachedOutputFile}");
					_ = Directory.CreateDirectory(args.CacheDir);
					if (File.Exists(cachedOutputFile))
						File.Delete(cachedOutputFile);
					File.Copy(dllPath, cachedOutputFile);
				}
				catch { }
			}
		}
		catch (Exception ex)
		{
			if (!string.IsNullOrEmpty(args.CacheDir))
			{
				string cachedOutputFile = Path.Combine(args.CacheDir, loader.OutputFilename);
				Console.WriteLine("Failed to generate, attempting to load from cache");
				if (!File.Exists(cachedOutputFile))
				{
					Console.Error.WriteLine(ex.ToString());
					Console.Error.WriteLine("Could not generate interop dll and none was present in cache");
					return 1;
				}

				if (File.Exists(args.OutputFile))
					File.Delete(args.OutputFile);
				Console.WriteLine($"Using cached artifact: {cachedOutputFile}");
				File.Copy(cachedOutputFile, args.OutputFile);
			}
			else
			{
				Console.Error.WriteLine(ex.ToString());
				Console.Error.WriteLine("Could not generate interop dll and none was present in cache");
				return 1;
			}
		}

		_ = Directory.CreateDirectory(args.OutputDir);
		string outputManifest = Path.Combine(args.OutputDir, Constants.ExtensionManifestFileName);
		if (File.Exists(outputManifest))
			File.Delete(outputManifest);
		manifest.RequireVersion ??= "3.0.0";
		manifest.CompatibleVersion ??= new Version(3, 0, 0);
		File.WriteAllText(outputManifest, JsonConvert.SerializeObject(manifest));
		return 0;
	}
}
