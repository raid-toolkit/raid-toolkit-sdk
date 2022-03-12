using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Raid.Toolkit.Model;

namespace Raid.Toolkit.Extensibility.Tasks
{
    public class Codegen : Microsoft.Build.Utilities.Task
    {
        [Required]
        public string OutputFile { get; set; }
        public override bool Execute()
        {
            Log.LogMessage($"Writing to {OutputFile}...");

            ModelLoader loader = new()
            {
                OutputDirectory = Path.GetDirectoryName(OutputFile)
            };
            List<Regex> patternMatchers = new();
            string dllPath = loader.Build(patternMatchers, false).GetResultSync();
            if (File.Exists(OutputFile))
                File.Delete(OutputFile);

            File.Move(dllPath, OutputFile);
            return true;
        }
    }
}
