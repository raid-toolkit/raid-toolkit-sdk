using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Common;
using Raid.Toolkit.UI;

namespace Raid.Toolkit
{
    [Verb("run", isDefault: true, HelpText = "Runs the service")]
    internal class RunOptions
    {
        [Option('s', "standalone", HelpText = "Runs in standalone mode")]
        public bool Standalone { get; set; }

        [Option('n', "no-ui", HelpText = "Runs without UI (only valid for standalone mode)")]
        public bool NoUI { get; set; }

        [Option('w', "wait", HelpText = "Wait <ms> for an existing instance to shut down before starting")]
        public int? Wait { get; set; }

        [Option('u', "post-update")]
        public bool Update { get; set; }

        [Option('d', "debug", Hidden = true)]
        public bool Debug { get; set; }
    }

    internal class RunTask : CommandTaskBase<RunOptions>
    {
        private readonly IServiceProvider ServiceProvider;
        public RunTask(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        protected override Task<int> Invoke(RunOptions options)
        {
            if (!options.Standalone)
            {
                options.NoUI = false;

                if (!RegistrySettings.IsInstalled ||
                    (!options.Debug && AppHost.ExecutablePath.ToLowerInvariant() != RegistrySettings.InstalledExecutablePath.ToLowerInvariant()))
                {
                    Application.Run(new InstallWindow());
                    return Task.FromResult(0);
                }
            }

            // kill existing processes for debugging
            if (options.Debug && Debugger.IsAttached)
                KillExistingProcesses();

            using (var mutex = new Mutex(false, "RaidToolkit Singleton"))
            {
                bool isAnotherInstanceOpen = !mutex.WaitOne(options.Wait.HasValue ? TimeSpan.FromMilliseconds(options.Wait.Value) : TimeSpan.Zero);
                if (isAnotherInstanceOpen && !options.Standalone)
                {
                    return Task.FromResult(1);
                }

                Application.Run(ActivatorUtilities.CreateInstance<MainWindow>(ServiceProvider, options));
            }

            return Task.FromResult(0);
        }

        private static void KillExistingProcesses()
        {
            var existingProcs = Process
                                .GetProcessesByName("Raid.Toolkit")
                                .Where(proc => proc.Id != Environment.ProcessId);

            foreach (var existingProc in existingProcs)
            {
                existingProc.Kill();
            }
        }
    }
}