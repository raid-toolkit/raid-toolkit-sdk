using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Newtonsoft.Json;
using Raid.Toolkit.Model;

namespace Raid.Toolkit.Extensibility.Tasks
{
    public class Codegen : Microsoft.Build.Utilities.Task
    {
        [Required]
        public string[]? ManifestFiles { get; set; }

        [Required]
        public string? OutputFile { get; set; }

        public override bool Execute()
        {
            Log.LogMessage($"Writing to {OutputFile}...");

            if (string.IsNullOrEmpty(OutputFile))
            {
                Log.LogError("OutputFile must be specified");
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

            ExtensionManifest manifest = JsonConvert.DeserializeObject<ExtensionManifest>(File.ReadAllText(ManifestFiles[0]))!;

            ModelLoader loader = new()
            {
                OutputDirectory = Path.GetDirectoryName(OutputFile)
            };
            List<Regex> patternMatchers = manifest.Codegen?.Types?
                .Select(pattern => new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled)).ToList()
                ?? new();
            string dllPath = loader.Build(patternMatchers, false).GetResultSync();
            if (File.Exists(OutputFile))
                File.Delete(OutputFile);

            File.Move(dllPath, OutputFile);

            return true;
        }
    }
}
