using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Raid.Toolkit.Model;

#if NET5_0_OR_GREATER
[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif

namespace Model.PreBuild
{
    internal class Program
    {
        private static readonly Regex[] SharedTypePatterns = new[] {
            new Regex(@"^Client\.Model\.AppModel$", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Client\.Model\.Gameplay\.Artifacts\.ExternalArtifactsStorage$", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Client\.Model\.Gameplay\.StaticData\.ClientStaticDataManager$", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Client\.RaidApp\.RaidApplication$", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Client\.RaidApp\.RaidViewMaster$", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Client\.View\.Views\.", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Client\.ViewModel\.AppViewModel$", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Client\.ViewModel\.Contextes\.", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Contexts$", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^ECS\.(Components|ViewModel)\.", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Entitas\.", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^SharedModel\.Meta\.Artifacts\.ArtifactStorage\.ArtifactStorageResolver$", RegexOptions.Singleline | RegexOptions.Compiled),
        };

        private static async Task Main(string[] args)
        {
            if (Environment.GetEnvironmentVariable("IS_CI") == "true")
            {
                return;
            }
            ModelLoader loader = new();
            string outputFile = await loader.Build(SharedTypePatterns, force: Environment.GetEnvironmentVariable("FORCE") == "true");
            string targetFile = Path.Combine(args[0], Path.GetFileName(outputFile));
            Console.WriteLine($"Copying [{outputFile}] to [{targetFile}]");
            _ = Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
            File.Delete(targetFile);
            File.Copy(outputFile, targetFile);
        }
    }
}
