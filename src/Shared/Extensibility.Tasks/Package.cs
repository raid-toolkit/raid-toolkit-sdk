using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using Microsoft.Build.Framework;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Raid.Toolkit.Extensibility.Tasks
{
    public class Package : Microsoft.Build.Utilities.Task
    {
        [Required]
        public string? OutputDir { get; set; }

        [Required]
        public string? OutputFile { get; set; }

        [Required]
        public string? Install { get; set; }

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

            string[] packagesToClean = Directory.GetFiles(OutputDir, "*.rtkx");
            foreach (string filePath in packagesToClean)
                File.Delete(filePath);

            string tempOutput = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("d"));
            ZipFile.CreateFromDirectory(OutputDir, tempOutput);
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
                using (var mutex = new Mutex(false, "RaidToolkit Singleton"))
                {
                    bool isRunning = !mutex.WaitOne(TimeSpan.Zero);
                    if (!isRunning)
                    {
                        mutex.ReleaseMutex();
                    }
                    else
                    {
                        Log.LogMessage(MessageImportance.High, $"Extension in use, will not deploy");
                        return;
                    }
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
}
