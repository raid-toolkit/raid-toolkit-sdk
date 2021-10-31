using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raid.Model;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace Raid.Service
{
    [Verb("run", isDefault: true, HelpText = "Runs the service")]
    public class RunOptions
    {
        [Option('s', "--standalone", HelpText = "Runs in standalone mode")]
        public bool Standalone { get; set; }

        [Option('n', "--no-ui", HelpText = "Runs without UI (only valid for standalone mode)")]
        public bool NoUI { get; set; }
    }
    static class RunAction
    {
        public static Task<int> Execute(RunOptions options)
        {
            if (!options.Standalone)
            {
                options.NoUI = false;
            }
            string exeDir = Path.GetDirectoryName(AppConfiguration.ExecutablePath);
            string parentDirName = Path.GetFileName(exeDir);
            if (Environment.GetEnvironmentVariable("RTK_DEBUG") != "true" && parentDirName != "win-x64")
            {
                string expectedInstallPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RaidToolkit");
                string exeName = Path.GetFileName(AppConfiguration.ExecutablePath);
                string expectedExePath = Path.Join(expectedInstallPath, exeName);
                string expectedConfigPath = Path.Join(expectedInstallPath, "appsettings.json");
                if (!AppConfiguration.ExecutablePath.Equals(expectedExePath, StringComparison.OrdinalIgnoreCase))
                {
                    Directory.CreateDirectory(expectedInstallPath);
                    File.Copy(AppConfiguration.ExecutablePath, expectedExePath, true);
                    File.Copy(Path.Join(exeDir, "appsettings.json"), expectedConfigPath, true);
                    Process.Start(expectedExePath, Environment.GetCommandLineArgs().Skip(1));
                    return Task.FromResult(0);
                }
            }
            using (var mutex = new Mutex(false, "RaidToolkit Singleton"))
            {
                bool isAnotherInstanceOpen = !mutex.WaitOne(TimeSpan.Zero);
                if (isAnotherInstanceOpen && !options.Standalone)
                {
                    return Task.FromResult(1);
                }

                try
                {
                    Run(options).Wait();
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            Application.ExitThread();
            return Task.FromResult(0);
        }

        private static Task Run(RunOptions options)
        {
            using (new ModelAssemblyResolver())
            {
                var host = RaidHost.CreateHost();
                var task = Task.Run(async () =>
                {
                    try
                    {
                        await host.RunAsync();
                    }
                    catch (OperationCanceledException) { }
                });
                RaidHost.Services.GetRequiredService<MainService>().Run(options);
                return task;
            }
        }
    }
}