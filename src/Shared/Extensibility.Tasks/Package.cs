using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Newtonsoft.Json;
using Raid.Toolkit.Model;

namespace Raid.Toolkit.Extensibility.Tasks
{
    public class Package : Microsoft.Build.Utilities.Task
    {
        [Required]
        public string? OutputDir { get; set; }

        [Required]
        public string? OutputFile { get; set; }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, $"Packaging extension");

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

            string[] packagesToClean = Directory.GetFiles(OutputDir, "*.rtkx");
            foreach (string filePath in packagesToClean)
                File.Delete(filePath);

            string tempOutput = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("d"));
            ZipFile.CreateFromDirectory(OutputDir, tempOutput);
            File.Move(tempOutput, OutputFile);
            return true;
        }
    }
}
